# Skill Registry

**Delegator use only.** Compact project rules for sub-agent prompts.

## User Skills

| Trigger | Skill | Path |
|---------|-------|------|
| Unity game feature, mechanic, system, game jam, or prototyping | unity-rapid-prototyping | `.agents/skills/unity-rapid-prototyping/SKILL.md` |
| Writing Go tests or Bubbletea TUI tests | go-testing | `C:\Users\luis_\.config\opencode\skills\go-testing\SKILL.md` |
| Implementing changes or splitting reviewable work units | work-unit-commits | `C:\Users\luis_\.config\opencode\skills\work-unit-commits\SKILL.md` |
| Writing review-facing documentation | cognitive-doc-design | `C:\Users\luis_\.config\opencode\skills\cognitive-doc-design\SKILL.md` |

## Compact Rules

### unity-rapid-prototyping
- Validate the core loop before architecture or polish.
- Apply the decision gate: core-loop value, <1h feasibility, 10x-simpler version, asset-over-custom, and finishability.
- Prefer one scene, plain MonoBehaviours, direct references, and placeholders; no speculative DI/event-bus architecture.
- Validate Unity MCP prerequisites and packages before building; check console after script changes.
- Validate logic with EditMode/PlayMode tests and gameplay with play mode/screenshots.
- If a mechanic is not fun after one focused iteration, kill or simplify it.

### work-unit-commits
- Commit by deliverable behavior/fix, not by file type.
- Keep tests and docs beside the behavior they verify/explain.

## Project Conventions

| File | Path | Notes |
|------|------|-------|
| Unity rapid-prototyping skill | `.agents/skills/unity-rapid-prototyping/SKILL.md` | Project-specific workflow and gameplay constraints |
| Unity package manifest | `Packages/manifest.json` | Package and test framework source of truth |
| Unity editor version | `ProjectSettings/ProjectVersion.txt` | Unity 6000.3.10f1 |

No project-level AGENTS.md, CLAUDE.md, GEMINI.md, .cursorrules, or copilot-instructions.md was found.
