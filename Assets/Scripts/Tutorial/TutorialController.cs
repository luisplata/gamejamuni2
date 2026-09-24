using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Scripted guided tutorial (Option A). Runs a 6-step ladder of
/// [text, objective, zone pulse, minHold, condition] records polled in Update:
/// each step blocks until its gameplay condition holds (plus a minimum hold),
/// then advances to the next narration. Built in Start — AFTER every Awake —
/// so HandController.CurrentLevel is already the tutorial's La Llorona level
/// (GameSession.tutorialLevel, design D1). The objective panel is built from
/// the level's star recipe. NO completion hook: the star-only recipe ends the
/// round through the stock win flow (ServeFood → WinRound → fade → next scene).
/// </summary>
public class TutorialController : MonoBehaviour
{
    /// <summary>Which zone to pulse when a step advances (visual hint).</summary>
    enum ZoneTarget { None, Center, Corner }

    /// <summary>One ladder rung: narration + objective + pulse + hold + gameplay condition.</summary>
    [Serializable]
    class Step
    {
        public string text;
        public string objective;
        public ZoneTarget pulse;
        public float minHold;
        public Func<bool> done;
    }

    [SerializeField] HandController handController;
    [SerializeField] EnemyView enemyView;
    [SerializeField] DropZone centerZone;
    [SerializeField] DropZone cornerZone;
    [SerializeField] Image centerImage;
    [SerializeField] Image cornerImage;
    [SerializeField] TMP_Text objectiveLabel;
    [SerializeField] RecipeData goalRecipe;

    readonly List<Step> _steps = new();
    string _goalLine;
    int _index;
    float _stepTime;
    bool _finished;

    void Start()
    {
        if (handController == null || enemyView == null || centerZone == null || cornerZone == null) return;
        var level = handController.CurrentLevel;
        if (level == null || level.recipes == null || level.recipes.Count == 0) return;

        // Objective panel: the star goal, driven entirely by data (design D3).
        if (goalRecipe == null) goalRecipe = level.recipes[0];
        _goalLine = $"Objetivo: {goalRecipe.icon} {goalRecipe.dishName} = {IngredientNames()}";
        if (objectiveLabel != null) objectiveLabel.text = _goalLine;

        BuildSteps();
        enemyView.Say(_steps[_index].text);
    }

    void Update()
    {
        if (_finished || _steps.Count == 0) return;
        _stepTime += Time.deltaTime;
        if (_steps[_index].done() && _stepTime >= _steps[_index].minHold)
        {
            if (_index == _steps.Count - 1) _finished = true; // final rung: panel stays
            else Advance();
        }
    }

    void Advance()
    {
        _stepTime = 0f;
        _index++;
        var s = _steps[_index];
        enemyView.Say(s.text);
        if (objectiveLabel != null) objectiveLabel.text = s.objective;
        StartCoroutine(PulseZone(s.pulse));
    }

    void BuildSteps()
    {
        _steps.Add(new Step
        {
            text = "Bienvenido, cocinero. Soy la Llorona y tengo antojo de TACOS DE CHAPULINES. Te enseño a prepararlos.",
            objective = _goalLine,
            pulse = ZoneTarget.None,
            minHold = 2.5f,
            done = () => true
        });

        _steps.Add(new Step
        {
            text = "Mira el color de cada carta: ROJO es base, VERDE es complemento y AMARILLO es sazón. Lleva cualquier carta a una zona.",
            objective = "Arrastra una carta a la ZONA AZUL (cocina) o a la ZONA ROJA (basura)",
            pulse = ZoneTarget.None,
            minHold = 0.4f,
            done = () => centerZone.Count + cornerZone.Count >= 1
        });

        _steps.Add(new Step
        {
            text = "La ZONA AZUL es la COCINA: ahí se arma el plato con hasta 3 cartas.",
            objective = "Coloca una carta en la ZONA AZUL (cocina)",
            pulse = ZoneTarget.Center,
            minHold = 0.4f,
            done = () => centerZone.Count >= 1
        });

        _steps.Add(new Step
        {
            text = "La ZONA ROJA es la BASURA. Tira una carta que no uses y pulsa REFILL para descartarla y llenar tu mano. ¡Los refills son limitados!",
            objective = "Tira una carta a la ZONA ROJA (basura) y pulsa REFILL",
            pulse = ZoneTarget.Corner,
            minHold = 0.4f,
            done = TrashThenRefill
        });

        _steps.Add(new Step
        {
            text = "Ahora arma el plato que te pedí. Solo se cocinan 3 cartas exactas.",
            objective = _goalLine,
            pulse = ZoneTarget.None,
            minHold = 0.4f,
            done = GoalParked
        });

        _steps.Add(new Step
        {
            text = "¡Perfecto! Cuando el plato esté listo, pulsa PREPARAR para cocinarlo.",
            objective = "Pulsa PREPARAR",
            pulse = ZoneTarget.None,
            minHold = 0.4f,
            done = () => handController.LiveDish != null
        });
    }

    /// <summary>Step 4: trash must be filled then dumped (REFILL) — saw-then-cleared.</summary>
    bool TrashThenRefill()
    {
        if (cornerZone.Count >= 1) _sawTrash = true;
        return _sawTrash && cornerZone.Count == 0;
    }
    bool _sawTrash;

    /// <summary>Step 5: the center cook queue is exactly the goal recipe's ingredient set.</summary>
    bool GoalParked()
    {
        if (goalRecipe == null || goalRecipe.ingredients == null || goalRecipe.ingredients.Count == 0) return false;
        if (centerZone.Count != goalRecipe.ingredients.Count) return false;
        var set = new HashSet<CardData>();
        foreach (var c in centerZone.Held)
            if (c != null && c.Data != null) set.Add(c.Data);
        return set.SetEquals(goalRecipe.ingredients);
    }

    string IngredientNames()
    {
        var names = new List<string>();
        if (goalRecipe != null && goalRecipe.ingredients != null)
            foreach (var ing in goalRecipe.ingredients)
                if (ing != null) names.Add(ing.displayName);
        return string.Join(" + ", names);
    }

    /// <summary>Flashes the zone image alpha so the player's eye finds the target zone.</summary>
    IEnumerator PulseZone(ZoneTarget target)
    {
        Image img = target == ZoneTarget.Center ? centerImage
                 : target == ZoneTarget.Corner ? cornerImage : null;
        if (img == null) yield break;
        float baseA = img.color.a;
        for (int i = 0; i < 3; i++)
        {
            yield return LerpAlpha(img, baseA, 0.9f, 0.22f);
            yield return LerpAlpha(img, 0.9f, baseA, 0.22f);
        }
    }

    IEnumerator LerpAlpha(Image img, float from, float to, float duration)
    {
        Color c = img.color;
        for (float p = 0f; p < 1f; p += Time.deltaTime / Mathf.Max(duration, 0.0001f))
        {
            c.a = Mathf.Lerp(from, to, Mathf.Clamp01(p));
            img.color = c;
            yield return null;
        }
        c.a = to;
        img.color = c;
    }
}