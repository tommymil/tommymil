import { existsSync, writeFileSync } from "node:fs";
import { resolve } from "node:path";

import AdmZip from "adm-zip";

function option(name, fallback = null) {
  const index = process.argv.indexOf(name);
  return index >= 0 && index + 1 < process.argv.length ? process.argv[index + 1] : fallback;
}

function inputBlockId(input, blocks) {
  if (!Array.isArray(input)) return null;
  for (const value of input.slice(1)) {
    if (typeof value === "string" && blocks[value]) return value;
  }
  return null;
}

function removeBlockTree(blocks, rootId) {
  const pending = [rootId];
  const visited = new Set();
  while (pending.length > 0) {
    const id = pending.pop();
    if (!id || visited.has(id) || !blocks[id]) continue;
    visited.add(id);
    const block = blocks[id];
    if (block.next) pending.push(block.next);
    for (const input of Object.values(block.inputs ?? {})) {
      const childId = inputBlockId(input, blocks);
      if (childId) pending.push(childId);
    }
  }
  for (const id of visited) delete blocks[id];
}

const finalPath = resolve(option("--final", "../../docs/showcase/projekty/02-Mega-Obby-FINAL.sb3"));
const startPath = resolve(option("--output", "../../docs/showcase/projekty/02-Mega-Obby-START.sb3"));
if (!existsSync(finalPath)) throw new Error(`Brak FINAL: ${finalPath}`);

const zip = new AdmZip(finalPath);
const entry = zip.getEntry("project.json");
if (!entry) throw new Error("FINAL nie zawiera project.json");

const project = JSON.parse(entry.getData().toString("utf8"));
const target = project.targets.find((item) => item.name === "Laser2");
if (!target) throw new Error("Brak duszka Laser2");

const topLevelIds = Object.entries(target.blocks)
  .filter(([, block]) => block.topLevel)
  .map(([id]) => id);
if (topLevelIds.length !== 2) throw new Error("Laser2 nie ma dwoch oczekiwanych skryptow");
for (const id of topLevelIds) removeBlockTree(target.blocks, id);

target.comments ??= {};
for (const [id, comment] of Object.entries(target.comments)) {
  if (comment.text?.includes("TU PRACUJE UCZEŃ")) delete target.comments[id];
}
target.comments.showcase_Laser2_full_patrol = {
  blockId: null,
  x: 20,
  y: 20,
  width: 400,
  height: 190,
  minimized: false,
  text: "TU PRACUJE UCZEŃ: zbuduj cały patrol Laser2 — start, pozycję, pokazanie, pętlę dwóch przelotów oraz reset pozycji po komunikacie POZIOM_2.",
};

zip.updateFile("project.json", Buffer.from(JSON.stringify(project), "utf8"));
writeFileSync(startPath, zip.toBuffer());
console.log(`Utworzono ${startPath}`);
