#!/usr/bin/env node
// Weekly listing refresh — keeps the catalog's title, description, and freshness
// markers current using a small rotation of precomputed copy. No AI / model at runtime.
//
// What it does:
//   1. README H1 (title): rotated to this week's variant.
//   2. "Last verified" badge + footer: set to the current month ONLY if the month changed.
//   3. "Updated" badge: refreshed to today's date every run.
//   4. Prints the repository About description + topics for a later step to apply.
//
// Preview locally (nothing is committed or pushed here):
//   node automation/weekly-update.mjs
//   git diff
//
// TODO(model): to generate fresh copy instead of rotating the list, replace the
//              variant lookup below with a model call returning { h1, description, topicsExtra }.

import { readFileSync, writeFileSync } from 'node:fs';
import { fileURLToPath } from 'node:url';
import { dirname, join } from 'node:path';

const here = dirname(fileURLToPath(import.meta.url));
const repoRoot = join(here, '..');
const variants = JSON.parse(readFileSync(join(here, 'listing-variants.json'), 'utf8'));

// --- pick this week's variant (anchored to a Monday, wraps after the list ends) ---
const anchor = new Date(variants.anchorMondayUtc + 'T00:00:00Z');
const now = new Date();
const msPerWeek = 7 * 24 * 60 * 60 * 1000;
const weeksSince = Math.floor((now - anchor) / msPerWeek);
const total = variants.weeks.length;
const idx = (((weeksSince % total) + total) % total); // 0-based, never negative
const v = variants.weeks[idx];

if (weeksSince >= total) {
  console.warn(
    `WARNING: week ${weeksSince + 1} — the ${total}-week list is exhausted and now repeating. ` +
    `Extend listing-variants.json or integrate a model for continued variety.`
  );
}

// --- mutate README in memory, write once ---
const readmePath = join(repoRoot, 'README.md');
let md = readFileSync(readmePath, 'utf8');

// 1) H1 title (first single-hash heading)
md = md.replace(/^# .*$/m, `# ${v.h1}`);

// 2) Last verified — month-gated (no-op if the month is unchanged)
const monthName = now.toLocaleString('en-US', { month: 'long', timeZone: 'UTC' });
const year = now.getUTCFullYear();
md = md.replace(/(Last%20verified-)[A-Za-z]+%20\d{4}/g, `$1${monthName}%20${year}`);
md = md.replace(/\*Last verified: [A-Za-z]+ \d{4}\*/g, `*Last verified: ${monthName} ${year}*`);

// 3) "Updated" badge — refreshed every run (shields.io renders `--` as `-`)
const stamp = now.toISOString().slice(0, 10).replace(/-/g, '--');
const updatedBadge = `![Updated](https://img.shields.io/badge/Updated-${stamp}-success?style=flat)`;
if (/!\[Updated\]\([^)]*\)/.test(md)) {
  md = md.replace(/!\[Updated\]\([^)]*\)/, updatedBadge);
} else {
  // insert once, right after the Last verified badge line
  md = md.replace(/(!\[Last verified\]\([^)]*\)\n)/, `$1${updatedBadge}\n`);
}

writeFileSync(readmePath, md);

// --- repository metadata: emit for a later/manual step ---
const topics = [...new Set([...variants.topicsCore, ...(v.topicsExtra || [])])].map((t) =>
  t.toLowerCase()
);

console.log(`week=${idx + 1}/${total}`);
console.log(`h1=${v.h1}`);
console.log(`description=${v.description}`);
console.log(`topics=${topics.join(',')}`);

if (process.env.GITHUB_OUTPUT) {
  const out =
    [`week=${idx + 1}`, `description=${v.description}`, `topics=${topics.join(',')}`].join('\n') +
    '\n';
  writeFileSync(process.env.GITHUB_OUTPUT, out, { flag: 'a' });
}
