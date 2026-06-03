# Weekly listing refresh

A small, transparent automation that keeps this catalog **current and easy to find**: it rotates the listing title and short description and refreshes the "updated" marker on a weekly cadence. It uses a precomputed list of variants — **no AI/model at runtime** — so it's predictable, free to run, and easy for contributors to edit.

## What changes each week

| Element | Behavior | Why |
|---|---|---|
| README **title** (H1) | Rotated to the week's variant | Keeps the catalog visibly current; varies the phrasing |
| Repository **About description** | Set to the week's variant | The short summary shown on the repo and in search results |
| Repository **topics** | `topicsCore` + the week's `topicsExtra` | Discoverability on GitHub |
| **"Updated" badge** | Refreshed to today's date every run | Visible freshness marker |
| **"Last verified"** badge + footer | Updated **only when the month changes** | Stays honest — no false "verified today" churn |

All copy lives in [`listing-variants.json`](listing-variants.json). It's plain text — **contributions to improve the titles and descriptions are welcome** (see the repo's `CONTRIBUTING.md`).

## Files

- `listing-variants.json` — the 10-week list of titles / descriptions / topics.
- `weekly-update.mjs` — applies the current week's variant to `README.md` and prints the description + topics.
- `../.github/workflows/weekly-refresh.yml` — the workflow (manual-trigger only for now).

## Preview locally (no commit, no push)

```bash
node automation/weekly-update.mjs   # edits README.md in place
git diff                            # review
git restore README.md               # undo (this also discards other uncommitted README edits)
```

## Enabling automation when you're ready

1. **Schedule** — uncomment the `schedule:` block in `../.github/workflows/weekly-refresh.yml`
   (`0 8 * * 1` = Mondays 15:00 Asia/Bangkok; GitHub cron is best-effort and may run late).
2. **Repository metadata** — to let it update the About description/topics, add a token with repo
   admin rights as the secret `REPO_ADMIN_TOKEN`. The default `GITHUB_TOKEN` can't edit repo
   metadata, so that step stays inert until the secret exists.
3. **Review flow** — the workflow opens a pull request (`chore/weekly-refresh`) rather than pushing
   to `main`. Merge to apply, or close to skip a week.

## Using a model later

Replace the variant lookup in `weekly-update.mjs` (and the corresponding workflow step) with a model
call that returns the same fields (`h1`, `description`, `topicsExtra`). Everything else — the README
edits, badge logic, and PR step — stays the same. Look for the `TODO(model)` markers.

## Notes

- After week 10 the list **wraps** and the script prints a warning; extend the JSON or add a model
  before then for continued variety.
- Keep titles accurate. Honest phrasing is the point of this catalog; the stable, keyword-aware
  description does more for discoverability than chasing title churn.
