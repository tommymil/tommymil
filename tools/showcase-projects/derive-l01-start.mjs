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

const finalPath = resolve(option("--final", "../../docs/showcase/projekty/01-Lowca-Skarbow-FINAL.sb3"));
const startPath = resolve(option("--output", "../../docs/showcase/projekty/01-Lowca-Skarbow-START.sb3"));

if (!existsSync(finalPath)) throw new Error(`Brak FINAL: ${finalPath}`);

const zip = new AdmZip(finalPath);
const entry = zip.getEntry("project.json");
if (!entry) throw new Error("FINAL nie zawiera project.json");

const project = JSON.parse(entry.getData().toString("utf8"));
const target = project.targets.find((item) => item.name === "nagroda rzadka");
if (!target) throw new Error("Brak duszka nagroda rzadka");

const collisionEntry = Object.entries(target.blocks).find(([, block]) =>
  block.opcode === "control_if" && inputBlockId(block.inputs?.SUBSTACK, target.blocks));
if (!collisionEntry) throw new Error("Brak warunku z reakcją rzadkiej nagrody");

const [collisionId, collision] = collisionEntry;
const reactionId = inputBlockId(collision.inputs.SUBSTACK, target.blocks);
removeBlockTree(target.blocks, reactionId);
delete collision.inputs.SUBSTACK;

target.comments ??= {};
for (const [id, comment] of Object.entries(target.comments)) {
  if (comment.text?.includes("TU PRACUJE UCZEŃ")) delete target.comments[id];
}
target.comments.showcase_nagroda_rzadka_SUBSTACK = {
  blockId: collisionId,
  x: 0,
  y: 0,
  width: 380,
  height: 170,
  minimized: false,
  text: "TU PRACUJE UCZEŃ: wyłącz aktywność, dodaj 3 punkty, zagraj Coin, nadaj komunikat 'rzadka +3', ukryj gwiazdę i dodaj krótką pauzę.",
};

zip.updateFile("project.json", Buffer.from(JSON.stringify(project), "utf8"));
writeFileSync(startPath, zip.toBuffer());

console.log(`Utworzono ${startPath}`);
