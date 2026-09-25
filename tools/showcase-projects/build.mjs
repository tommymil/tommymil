import { createHash } from "node:crypto";
import { existsSync, mkdirSync, readFileSync, readdirSync, writeFileSync, copyFileSync } from "node:fs";
import { basename, join, resolve } from "node:path";

import { DOMParser, XMLSerializer } from "@xmldom/xmldom";
import AdmZip from "adm-zip";
import lzma from "lzma";

function option(name, fallback = null) {
  const index = process.argv.indexOf(name);
  return index >= 0 && index + 1 < process.argv.length ? process.argv[index + 1] : fallback;
}

const sourceRoot = resolve(option("--source", "D:/moje/kodziaki-konspekty"));
const outputRoot = resolve(option("--output", "docs/showcase/projekty"));
const scratchRoot = join(sourceRoot, "SCRATCH GOTOWE");
const minecraftRoot = join(sourceRoot, "MINECRAFT GOTOWE");

if (!existsSync(scratchRoot) || !existsSync(minecraftRoot)) {
  throw new Error(`Brak katalogow zrodlowych w ${sourceRoot}`);
}

mkdirSync(outputRoot, { recursive: true });

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

function clearScratchSubstack(target, block, inputName, comment) {
  const rootId = inputBlockId(block.inputs?.[inputName], target.blocks);
  if (!rootId) throw new Error(`Brak ${inputName} w duszku ${target.name}`);

  removeBlockTree(target.blocks, rootId);
  delete block.inputs[inputName];

  const commentId = `showcase_${target.name.replace(/\W+/g, "_")}_${inputName}`;
  target.comments[commentId] = {
    blockId: Object.entries(target.blocks).find(([, value]) => value === block)?.[0] ?? null,
    x: 0,
    y: 0,
    width: 360,
    height: 150,
    minimized: false,
    text: comment,
  };
}

function scratchProject(sourceName, baseName, cut, prepare = null) {
  const sourcePath = join(scratchRoot, sourceName);
  const finalPath = join(outputRoot, `${baseName}-FINAL.sb3`);
  const startPath = join(outputRoot, `${baseName}-START.sb3`);

  const zip = new AdmZip(sourcePath);
  const projectEntry = zip.getEntry("project.json");
  if (!projectEntry) throw new Error(`Brak project.json w ${sourceName}`);

  const project = JSON.parse(projectEntry.getData().toString("utf8"));
  if (prepare) {
    prepare(project);
    zip.updateFile("project.json", Buffer.from(JSON.stringify(project), "utf8"));
    zip.writeZip(finalPath);
  } else {
    copyFileSync(sourcePath, finalPath);
  }

  cut(project);
  zip.updateFile("project.json", Buffer.from(JSON.stringify(project), "utf8"));
  zip.writeZip(startPath);

  return { startPath, finalPath };
}

function cutRareReward(project) {
  const target = project.targets.find((item) => item.name === "nagroda rzadka");
  if (!target) throw new Error("Brak duszka nagroda rzadka");

  const collision = Object.values(target.blocks).find(
    (block) => block.opcode === "control_if" && inputBlockId(block.inputs?.SUBSTACK, target.blocks),
  );

  if (!collision) {
    throw new Error("Nie znaleziono reakcji rzadkiej nagrody na dotkniecie");
  }

  clearScratchSubstack(
    target,
    collision,
    "SUBSTACK",
    "TU PRACUJE UCZEŃ: wyłącz aktywność, dodaj 3 punkty, zagraj Coin, nadaj komunikat 'rzadka +3', ukryj gwiazdę i dodaj krótką pauzę.",
  );
}

function cutMovingLaser(project) {
  const target = project.targets.find((item) => item.name === "Laser2");
  if (!target) throw new Error("Brak duszka Laser2");

  const topLevelIds = Object.entries(target.blocks)
    .filter(([, block]) => block.topLevel)
    .map(([id]) => id);
  if (topLevelIds.length !== 2) throw new Error("Laser2 nie ma dwoch oczekiwanych skryptow");

  for (const id of topLevelIds) removeBlockTree(target.blocks, id);

  target.comments ??= {};
  target.comments.showcase_Laser2_full_patrol = {
    blockId: null,
    x: 20,
    y: 20,
    width: 400,
    height: 190,
    minimized: false,
    text: "TU PRACUJE UCZEŃ: zbuduj cały patrol Laser2 — start, pozycję, pokazanie, pętlę dwóch przelotów oraz reset pozycji po komunikacie POZIOM_2.",
  };
}

function cutSuperAttack(project) {
  const target = project.targets.find((item) => item.name === "Pocisk");
  if (!target) throw new Error("Brak duszka Pocisk");

  const event = Object.values(target.blocks).find(
    (block) => block.topLevel
      && block.opcode === "event_whenbroadcastreceived"
      && block.fields?.BROADCAST_OPTION?.[0] === "SUPER_ATAK",
  );
  const condition = event && target.blocks[event.next];
  if (!condition || condition.opcode !== "control_if") {
    throw new Error("Nie znaleziono obslugi SUPER_ATAK");
  }

  clearScratchSubstack(
    target,
    condition,
    "SUBSTACK",
    "TU PRACUJE UCZEŃ: zbuduj serię ośmiu klonów obracanych co 45 stopni.",
  );
}

function setScratchNumber(target, blockId, inputName, value) {
  const input = target.blocks[blockId]?.inputs?.[inputName];
  if (!Array.isArray(input) || !Array.isArray(input[1])) {
    throw new Error(`Brak pola ${blockId}.${inputName} w projekcie Scratch`);
  }
  input[1][1] = String(value);
}

function prepareTntArena(project) {
  const stage = project.targets.find((item) => item.isStage);
  const tnt = project.targets.find((item) => item.name === "TNT");
  if (!stage || !tnt) throw new Error("Brak sceny lub duszka TNT w TNT Arena");

  setScratchNumber(stage, "L2_gt_0016", "OPERAND2", 10);
  setScratchNumber(stage, "L2_gt_0028", "OPERAND2", 18);
  setScratchNumber(stage, "L2_trans_wait_0023", "DURATION", 3);
  setScratchNumber(tnt, "L2_random_0126", "FROM", 1);
  setScratchNumber(tnt, "L2_random_0126", "TO", 4);
}

function cutTntArena(project) {
  const target = project.targets.find((item) => item.name === "TNT");
  const randomHazard = target?.blocks?.L2_randiflevel_0123;
  if (!target || !randomHazard || randomHazard.opcode !== "control_if") {
    throw new Error("Nie znaleziono losowego znikania pól w TNT Arena");
  }

  clearScratchSubstack(
    target,
    randomHazard,
    "SUBSTACK",
    "TU PRACUJE UCZEŃ: dodaj losowe opóźnienie, zabezpiecz pole z diamentem i zanim ukryjesz klon, pokaż animację TNT.",
  );
}

function decompress(buffer) {
  return new Promise((resolvePromise, rejectPromise) => {
    lzma.decompress(buffer, (result, error) => {
      if (error) rejectPromise(error);
      else resolvePromise(String(result));
    });
  });
}

function compress(text) {
  return new Promise((resolvePromise, rejectPromise) => {
    lzma.compress(text, 7, (result, error) => {
      if (error) rejectPromise(error);
      else resolvePromise(Buffer.from(result));
    });
  });
}

function replaceTsFunction(source, name, replacement) {
  const match = new RegExp(`function\\s+${name}\\s*\\(`).exec(source);
  if (!match) throw new Error(`Brak funkcji TypeScript ${name}`);

  const open = source.indexOf("{", match.index);
  let depth = 0;
  for (let index = open; index < source.length; index++) {
    if (source[index] === "{") depth++;
    if (source[index] === "}") depth--;
    if (depth === 0) {
      return source.slice(0, match.index) + replacement.trimEnd() + source.slice(index + 1);
    }
  }

  throw new Error(`Nie domknieto funkcji TypeScript ${name}`);
}

function replacePythonFunction(source, name, replacement) {
  const match = new RegExp(`^def\\s+${name}\\s*\\(`, "m").exec(source);
  if (!match) throw new Error(`Brak funkcji Python ${name}`);

  const separator = source.indexOf("\n# =================================", match.index);
  if (separator < 0) throw new Error(`Brak separatora po funkcji Python ${name}`);

  return source.slice(0, match.index) + replacement.trimEnd() + "\n\n" + source.slice(separator + 1);
}

function replaceBetween(source, startText, endText, replacement) {
  const start = source.indexOf(startText);
  const end = source.indexOf(endText, start + startText.length);
  if (start < 0 || end < 0) throw new Error(`Nie znaleziono zakresu ${startText}`);
  return source.slice(0, start) + replacement.trimEnd() + "\n" + source.slice(end);
}

function childElements(node, name) {
  return Array.from(node.childNodes ?? []).filter(
    (child) => child.nodeType === 1 && (!name || child.localName === name),
  );
}

function descendants(node, name) {
  return Array.from(node.getElementsByTagName(name));
}

function functionDefinition(document, name) {
  return descendants(document, "block").find((block) => {
    if (block.getAttribute("type") !== "function_definition") return false;
    const mutation = childElements(block, "mutation")[0];
    return mutation?.getAttribute("name") === name;
  });
}

function addBlockComment(document, block, text) {
  for (const old of childElements(block, "comment")) block.removeChild(old);
  const comment = document.createElementNS(block.namespaceURI, "comment");
  comment.setAttribute("pinned", "false");
  comment.setAttribute("h", "120");
  comment.setAttribute("w", "360");
  comment.appendChild(document.createTextNode(text));
  block.insertBefore(comment, block.firstChild);
}

function serializeBlocks(document) {
  return new XMLSerializer().serializeToString(document);
}

function cutFortressWaveXml(xml) {
  const document = new DOMParser().parseFromString(xml, "application/xml");
  const definition = functionDefinition(document, "fala");
  if (!definition) throw new Error("Brak bloczkowej funkcji fala");

  const condition = descendants(definition, "block").find((block) => block.getAttribute("type") === "controls_if");
  const body = condition && childElements(condition, "statement").find((item) => item.getAttribute("name") === "DO0");
  if (!condition || !body) throw new Error("Brak ciala bloczkowej funkcji fala");

  condition.removeChild(body);
  addBlockComment(document, condition, "TU PRACUJE UCZEŃ: komunikat, pętla/spawn zombie i szkieletów oraz odstęp czasu.");
  return serializeBlocks(document);
}

function cutLightningSwordXml(xml) {
  const document = new DOMParser().parseFromString(xml, "application/xml");
  const event = childElements(document.documentElement, "block").find(
    (block) => block.getAttribute("type") === "minecraftOnItemInteracted",
  );
  const conditions = event && descendants(event, "block").filter(
    (block) => block.getAttribute("type") === "controls_if",
  );
  const inner = conditions?.[1];
  const body = inner && childElements(inner, "statement").find((item) => item.getAttribute("name") === "DO0");
  if (!inner || !body) throw new Error("Brak ciala Piorunowego Miecza w Blocks");

  inner.removeChild(body);
  addBlockComment(document, inner, "TU PRACUJE UCZEŃ: zablokuj moc, przywołaj piorun, odczekaj i odblokuj moc.");
  return serializeBlocks(document);
}

function cutPyramidPathXml(xml) {
  const document = new DOMParser().parseFromString(xml, "application/xml");
  const definition = functionDefinition(document, "korytarz_pulapek");
  if (!definition) throw new Error("Brak bloczkowej funkcji korytarz_pulapek");

  const safePath = descendants(definition, "block").find((block) => block.getAttribute("type") === "controls_if");
  if (!safePath) throw new Error("Brak warunku bezpiecznego zygzaka");

  const wrapper = safePath.parentNode;
  const successorWrapper = childElements(safePath, "next")[0];
  const successor = successorWrapper && childElements(successorWrapper, "block")[0];
  if (wrapper?.localName !== "next" || !successor) {
    throw new Error("Nie mozna wyciac warunku bezpiecznego zygzaka");
  }

  wrapper.replaceChild(successor, safePath);
  const magmaStep = descendants(definition, "block").filter(
    (block) => block.getAttribute("type") === "minecraftFill",
  ).at(-1);
  addBlockComment(document, magmaStep, "TU PRACUJE UCZEŃ: po tym bloku dodaj warunek parzystości i bezpieczny zygzak z jasnych bloków.");
  return serializeBlocks(document);
}

function updateProjectIdentity(pkg, files, name, editor = null) {
  pkg.meta.name = name;
  if (editor) pkg.meta.editor = editor;

  const config = JSON.parse(files["pxt.json"]);
  config.name = name;
  if (editor) config.preferredEditor = editor;
  files["pxt.json"] = JSON.stringify(config, null, 4);
}

async function minecraftPair(sourceName, baseName, displayName, configure) {
  const sourcePath = join(minecraftRoot, sourceName);
  const packed = JSON.parse(await decompress(readFileSync(sourcePath)));
  const originalFiles = JSON.parse(packed.source);

  for (const variant of ["FINAL", "START"]) {
    const pkg = structuredClone(packed);
    const files = structuredClone(originalFiles);
    const state = { pkg, files, variant };
    configure(state);
    updateProjectIdentity(pkg, files, `${displayName} ${variant}`, state.editor ?? null);
    pkg.source = JSON.stringify(files);

    const outputPath = join(outputRoot, `${baseName}-${variant}.mkcd`);
    writeFileSync(outputPath, await compress(JSON.stringify(pkg, null, 2)));
  }
}

const fortressTsStart = `function fala (id: number) {
    if (graTrwa && id == numerGry) {
        // TU PRACUJE UCZEŃ:
        // dodaj komunikat oraz falę zombie i szkieletów.
    }
}`;

const lightningTsStart = `player.onItemInteracted(DIAMOND_SWORD, function () {
    if (gra_trwa == 1) {
        if (gotowa_moc == 1) {
            // TU PRACUJE UCZEŃ:
            // zablokuj moc, przywołaj piorun, odczekaj i odblokuj moc.
        } else {
            player.say("Moc sie laduje!")
        }
    } else {
        player.say("Najpierw uruchom gre!")
    }
})`;

const pyramidPyStart = `def korytarz_pulapek():

    blocks.fill(
        AIR,
        punkt(-1, 2, 0),
        punkt(1, 7, 3),
        FillOperation.REPLACE
    )

    for i in range(3):
        wysokosc = 1 + i
        droga = i

        blocks.fill(
            MAGMA_BLOCK,
            punkt(-1, wysokosc, droga),
            punkt(1, wysokosc, droga),
            FillOperation.REPLACE
        )

        # TU PRACUJE UCZEŃ:
        # dodaj warunek parzystości i bezpieczny zygzak.

        loops.pause(200)`;

const pyramidTsStart = `function korytarz_pulapek() {
    let wysokosc: number;
    let droga: number;
    blocks.fill(AIR, punkt(-1, 2, 0), punkt(1, 7, 3), FillOperation.Replace)
    for (let i = 0; i < 3; i++) {
        wysokosc = 1 + i
        droga = i
        blocks.fill(MAGMA_BLOCK, punkt(-1, wysokosc, droga), punkt(1, wysokosc, droga), FillOperation.Replace)
        // TU PRACUJE UCZEŃ:
        // dodaj warunek parzystości i bezpieczny zygzak.
        loops.pause(200)
    }
}`;

const scannerPyStart = `def sprawdz_skarb():

    global skarby

    # TU PRACUJE UCZEŃ:
    # rozpoznaj GOLD_BLOCK i DIAMOND_BLOCK,
    # a potem zwieksz licznik skarbow.
    pass`;

const scannerTsStart = `function sprawdz_skarb() {
    // TU PRACUJE UCZEŃ:
    // rozpoznaj GOLD_BLOCK i DIAMOND_BLOCK i zwieksz licznik.
}`;

const icePyStart = `def moc_lodu():

    global energia
    global barykada_aktywna
    global czas_barykady

    if energia >= 20:

        if barykada_aktywna == 0:

            energia -= 20
            barykada_aktywna = 1

            gameplay.title(
                mobs.target(ALL_PLAYERS),
                "LODOWA TARCZA!",
                "ZBUDUJ BARYKADE!"
            )

            # TU PRACUJE UCZEŃ:
            # zbuduj sciane z PACKED_ICE i dodaj dwie lampy.

            czas_barykady = 50
            player.say("LODOWA TARCZA!")
            pokaz_energie()

        else:
            player.say("Tarcza juz dziala!")

    else:
        player.say("Za malo energii!")`;

const iceTsStart = `function moc_lodu() {
    if (energia >= 20) {
        if (barykada_aktywna == 0) {
            energia -= 20
            barykada_aktywna = 1
            gameplay.title(mobs.target(ALL_PLAYERS), "LODOWA TARCZA!", "ZBUDUJ BARYKADE!")
            // TU PRACUJE UCZEŃ:
            // zbuduj sciane z PACKED_ICE i dodaj dwie lampy.
            czas_barykady = 50
            player.say("LODOWA TARCZA!")
            pokaz_energie()
        } else {
            player.say("Tarcza juz dziala!")
        }
    } else {
        player.say("Za malo energii!")
    }
}`;

function islandChestPython(withLoot) {
  if (!withLoot) {
    return `def skrzynia_wyspy():

    # TU PRACUJE UCZEŃ:
    # oblicz pozycje, postaw skrzynie i umiesc w niej trzy nagrody.
    pass`;
  }

  return `def skrzynia_wyspy():

    x = WYSPA_X
    y = WYSPA_Y
    z = WYSPA_Z - 3

    blocks.place(
        CHEST,
        world(x, y, z)
    )

    wloz_do_skrzyni(x, y, z, 0, "minecraft:diamond_sword", 1)
    wloz_do_skrzyni(x, y, z, 1, "minecraft:ender_pearl", 8)
    wloz_do_skrzyni(x, y, z, 2, "minecraft:golden_apple", 3)`;
}

function islandChestTs(withLoot) {
  if (!withLoot) {
    return `function skrzynia_wyspy() {
    // TU PRACUJE UCZEŃ:
    // oblicz pozycje, postaw skrzynie i umiesc w niej trzy nagrody.
}`;
  }

  return `function skrzynia_wyspy() {
    let x = WYSPA_X
    let y = WYSPA_Y
    let z = WYSPA_Z - 3
    blocks.place(CHEST, world(x, y, z))
    wloz_do_skrzyni(x, y, z, 0, "minecraft:diamond_sword", 1)
    wloz_do_skrzyni(x, y, z, 1, "minecraft:ender_pearl", 8)
    wloz_do_skrzyni(x, y, z, 2, "minecraft:golden_apple", 3)
}`;
}

const scratch = [
  scratchProject("Łowca Skarbów Final.sb3", "01-Lowca-Skarbow", cutRareReward),
  scratchProject("Mega Obby Final.sb3", "02-Mega-Obby", cutMovingLaser),
  scratchProject("Arena walki Final.sb3", "03-Arena-Walki", cutSuperAttack),
  scratchProject("TNT Arena.sb3", "10-TNT-Arena", cutTntArena, prepareTntArena),
];

await minecraftPair("minecraft-Magiczna-Forteca-Final.mkcd", "04-Magiczna-Forteca", "Magiczna Forteca", ({ files, variant }) => {
  if (variant === "START") {
    files["main.blocks"] = cutFortressWaveXml(files["main.blocks"]);
    files["main.ts"] = replaceTsFunction(files["main.ts"], "fala", fortressTsStart);
  }
});

await minecraftPair("minecraft-Arena-Łowców-Final.mkcd", "05-Arena-Lowcow", "Arena Łowców", ({ files, variant }) => {
  if (variant === "START") {
    files["main.blocks"] = cutLightningSwordXml(files["main.blocks"]);
    files["main.ts"] = replaceBetween(
      files["main.ts"],
      "player.onItemInteracted(DIAMOND_SWORD",
      "// =========================================\n// LICZNIK POKONANYCH",
      lightningTsStart,
    );
  }
});

await minecraftPair("minecraft-Złota-Piramida-Final.mkcd", "06-Zlota-Piramida", "Złota Piramida", (state) => {
  state.editor = "blocksprj";
  if (state.variant === "START") {
    state.files["main.blocks"] = cutPyramidPathXml(state.files["main.blocks"]);
    state.files["main.py"] = replacePythonFunction(state.files["main.py"], "korytarz_pulapek", pyramidPyStart);
    state.files["main.ts"] = replaceTsFunction(state.files["main.ts"], "korytarz_pulapek", pyramidTsStart);
  }
});

await minecraftPair("minecraft-Agent-Górnik-Final.mkcd", "07-Agent-Gornik", "Agent Górnik", ({ files, variant }) => {
  if (variant === "START") {
    files["main.py"] = replacePythonFunction(files["main.py"], "sprawdz_skarb", scannerPyStart);
    files["main.ts"] = replaceTsFunction(files["main.ts"], "sprawdz_skarb", scannerTsStart);
  }
});

await minecraftPair("minecraft-Arena-Żywiołów.mkcd", "08-Arena-Zywiolow", "Arena Żywiołów", ({ files, variant }) => {
  if (variant === "START") {
    files["main.py"] = replacePythonFunction(files["main.py"], "moc_lodu", icePyStart);
    files["main.ts"] = replaceTsFunction(files["main.ts"], "moc_lodu", iceTsStart);
  }
});

await minecraftPair("minecraft-Podniebna-Baza.mkcd", "09-Podniebna-Baza", "Podniebna Baza", ({ files, variant }) => {
  files["main.py"] = replacePythonFunction(files["main.py"], "skrzynia_wyspy", islandChestPython(variant === "FINAL"));
  files["main.ts"] = replaceTsFunction(files["main.ts"], "skrzynia_wyspy", islandChestTs(variant === "FINAL"));
});

function sha256(path) {
  return createHash("sha256").update(readFileSync(path)).digest("hex");
}

const generated = readdirSync(outputRoot)
  .filter((name) => /-(START|FINAL)\.(sb3|mkcd)$/.test(name))
  .sort((a, b) => a.localeCompare(b, "pl"));

if (generated.length !== 20) {
  throw new Error(`Oczekiwano 20 projektow, zapisano ${generated.length}`);
}

for (const name of generated) {
  const path = join(outputRoot, name);
  console.log(`${name}\t${readFileSync(path).length} B\t${sha256(path).slice(0, 12)}`);
}

if (scratch.length !== 4) throw new Error("Nie zbudowano czterech par Scratch");
