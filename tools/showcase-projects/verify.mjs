import { createHash } from "node:crypto";
import { existsSync, readFileSync, readdirSync } from "node:fs";
import { join, resolve } from "node:path";

import { DOMParser } from "@xmldom/xmldom";
import AdmZip from "adm-zip";
import lzma from "lzma";

function option(name, fallback) {
  const index = process.argv.indexOf(name);
  return index >= 0 && index + 1 < process.argv.length ? process.argv[index + 1] : fallback;
}

function assert(condition, message) {
  if (!condition) throw new Error(message);
}

function decompress(buffer) {
  return new Promise((resolvePromise, rejectPromise) => {
    lzma.decompress(buffer, (result, error) => {
      if (error) rejectPromise(error);
      else resolvePromise(String(result));
    });
  });
}

function hash(path) {
  return createHash("sha256").update(readFileSync(path)).digest("hex");
}

const outputRoot = resolve(option("--output", "../../docs/showcase/projekty"));
assert(existsSync(outputRoot), `Brak katalogu ${outputRoot}`);

const projects = readdirSync(outputRoot)
  .filter((name) => /-(START|FINAL)\.(sb3|mkcd)$/.test(name))
  .sort((a, b) => a.localeCompare(b, "pl"));
assert(projects.length === 20, `Oczekiwano 20 paczek, znaleziono ${projects.length}`);

for (const startName of projects.filter((name) => name.includes("-START."))) {
  const finalName = startName.replace("-START.", "-FINAL.");
  const startPath = join(outputRoot, startName);
  const finalPath = join(outputRoot, finalName);
  assert(projects.includes(finalName), `Brak pary FINAL dla ${startName}`);
  assert(hash(startPath) !== hash(finalPath), `${startName} jest kopią FINAL`);
}

for (const startName of projects.filter((name) => name.endsWith("-START.sb3"))) {
  const finalName = startName.replace("-START.", "-FINAL.");
  const start = JSON.parse(new AdmZip(join(outputRoot, startName)).readAsText("project.json"));
  const final = JSON.parse(new AdmZip(join(outputRoot, finalName)).readAsText("project.json"));
  const startComments = start.targets.flatMap((target) => Object.values(target.comments ?? {}));
  const finalComments = final.targets.flatMap((target) => Object.values(target.comments ?? {}));
  assert(startComments.filter((comment) => comment.text.includes("TU PRACUJE UCZEŃ")).length === 1,
    `${startName} nie ma dokładnie jednego oznaczonego zadania`);
  assert(finalComments.every((comment) => !comment.text.includes("TU PRACUJE UCZEŃ")),
    `${finalName} zawiera oznaczenie ze START`);
}

const obbyFinal = JSON.parse(new AdmZip(join(outputRoot, "02-Mega-Obby-FINAL.sb3")).readAsText("project.json"));
const obbyStart = JSON.parse(new AdmZip(join(outputRoot, "02-Mega-Obby-START.sb3")).readAsText("project.json"));
const obbyFinalLaser = obbyFinal.targets.find((item) => item.name === "Laser2");
const obbyStartLaser = obbyStart.targets.find((item) => item.name === "Laser2");
assert(obbyFinalLaser && obbyStartLaser, "Mega Obby: brak duszka Laser2");
assert(Object.keys(obbyStartLaser.blocks ?? {}).length === 0,
  "Mega Obby START: Laser2 powinien czekac na zbudowanie obu skryptow od zera");
assert(Object.values(obbyFinalLaser.blocks).filter((block) => block.topLevel).length === 2,
  "Mega Obby FINAL: Laser2 powinien miec dwa skrypty zdarzen");
assert(Object.values(obbyFinalLaser.blocks).filter((block) => block.opcode === "motion_glidesecstoxy").length === 2,
  "Mega Obby FINAL: patrol Laser2 powinien miec dwa przeloty");
assert(Object.values(obbyFinalLaser.blocks).some((block) =>
  block.opcode === "event_whenbroadcastreceived" && block.fields?.BROADCAST_OPTION?.[0] === "POZIOM_2"),
"Mega Obby FINAL: brak resetu Laser2 po komunikacie POZIOM_2");

function scratchNumber(project, targetName, blockId, inputName) {
  const target = project.targets.find((item) => item.name === targetName || (targetName === "Stage" && item.isStage));
  return target?.blocks?.[blockId]?.inputs?.[inputName]?.[1]?.[1];
}

const tntFinal = JSON.parse(new AdmZip(join(outputRoot, "10-TNT-Arena-FINAL.sb3")).readAsText("project.json"));
const tntStart = JSON.parse(new AdmZip(join(outputRoot, "10-TNT-Arena-START.sb3")).readAsText("project.json"));
assert(scratchNumber(tntFinal, "Stage", "L2_gt_0016", "OPERAND2") === "10", "TNT Arena: poziom 1 nie trwa 10 sekund");
assert(scratchNumber(tntFinal, "Stage", "L2_gt_0028", "OPERAND2") === "18", "TNT Arena: poziom 2 nie trwa 5 sekund po pauzie");
assert(scratchNumber(tntFinal, "Stage", "L2_trans_wait_0023", "DURATION") === "3", "TNT Arena: brak 3-sekundowej pauzy między poziomami");
assert(scratchNumber(tntFinal, "TNT", "L2_random_0126", "FROM") === "1", "TNT Arena: błędny początek losowego opóźnienia");
assert(scratchNumber(tntFinal, "TNT", "L2_random_0126", "TO") === "4", "TNT Arena: błędny koniec losowego opóźnienia");
assert(tntFinal.targets.find((item) => item.name === "TNT")?.blocks?.L2_randiflevel_0123?.inputs?.SUBSTACK,
  "TNT Arena FINAL: brak mechanizmu znikania pól");
assert(!tntStart.targets.find((item) => item.name === "TNT")?.blocks?.L2_randiflevel_0123?.inputs?.SUBSTACK,
  "TNT Arena START: mechanizm znikania nie został wycięty");

const minecraftEditors = new Map([
  ["04", "blocksprj"], ["05", "blocksprj"], ["06", "blocksprj"],
  ["07", "pyprj"], ["08", "pyprj"], ["09", "pyprj"],
]);
const minecraftSources = new Map();

for (const name of projects.filter((item) => item.endsWith(".mkcd"))) {
  const packed = JSON.parse(await decompress(readFileSync(join(outputRoot, name))));
  const files = JSON.parse(packed.source);
  minecraftSources.set(name, files);
  const config = JSON.parse(files["pxt.json"]);
  const expectedEditor = minecraftEditors.get(name.slice(0, 2));
  assert(packed.meta.editor === expectedEditor, `${name}: błędny edytor w meta`);
  assert(config.preferredEditor === expectedEditor, `${name}: błędny preferredEditor`);

  if (expectedEditor === "blocksprj") {
    const document = new DOMParser().parseFromString(files["main.blocks"], "application/xml");
    assert(document.getElementsByTagName("parsererror").length === 0, `${name}: niepoprawny XML Blocks`);
  }

  const primarySource = expectedEditor === "blocksprj" ? files["main.blocks"] : files["main.py"];
  if (name.includes("-START.")) {
    assert(primarySource.includes("TU PRACUJE UCZEŃ"), `${name}: brak oznaczenia zadania`);
  } else {
    assert(!primarySource.includes("TU PRACUJE UCZEŃ"), `${name}: FINAL zawiera oznaczenie zadania`);
  }
}

const islandStartPy = minecraftSources.get("09-Podniebna-Baza-START.mkcd")?.["main.py"] ?? "";
const islandFinalPy = minecraftSources.get("09-Podniebna-Baza-FINAL.mkcd")?.["main.py"] ?? "";
assert(islandStartPy.includes("def skrzynia_wyspy():\n\n    # TU PRACUJE UCZEŃ:"),
  "Podniebna Baza START: funkcja skrzyni nie jest przygotowana do zbudowania od zera");
assert(!islandStartPy.includes("wloz_do_skrzyni(x, y, z, 0, \"minecraft:diamond_sword\", 1)"),
  "Podniebna Baza START: wyposażenie skrzyni nie zostało wycięte");
for (const line of [
  "x = WYSPA_X",
  "y = WYSPA_Y",
  "z = WYSPA_Z - 3",
  "blocks.place(",
  "wloz_do_skrzyni(x, y, z, 0, \"minecraft:diamond_sword\", 1)",
  "wloz_do_skrzyni(x, y, z, 1, \"minecraft:ender_pearl\", 8)",
  "wloz_do_skrzyni(x, y, z, 2, \"minecraft:golden_apple\", 3)",
]) {
  assert(islandFinalPy.includes(line), `Podniebna Baza FINAL: brak ${line}`);
}

console.log("OK: 10 par START/FINAL, 4 Scratch, 3 Minecraft Blocks i 3 Minecraft Python.");
