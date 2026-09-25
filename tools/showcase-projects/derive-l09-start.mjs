import { existsSync, readFileSync, writeFileSync } from "node:fs";
import { resolve } from "node:path";

import lzma from "lzma";

function option(name, fallback = null) {
  const index = process.argv.indexOf(name);
  return index >= 0 && index + 1 < process.argv.length ? process.argv[index + 1] : fallback;
}

function decompress(buffer) {
  return new Promise((resolvePromise, rejectPromise) => {
    lzma.decompress(buffer, (result, error) => error ? rejectPromise(error) : resolvePromise(String(result)));
  });
}

function compress(text) {
  return new Promise((resolvePromise, rejectPromise) => {
    lzma.compress(text, 7, (result, error) => error ? rejectPromise(error) : resolvePromise(Buffer.from(result)));
  });
}

function replacePythonFunction(source, name, replacement) {
  const match = new RegExp(`^def\\s+${name}\\s*\\(`, "m").exec(source);
  if (!match) throw new Error(`Brak funkcji Python ${name}`);
  const separator = source.indexOf("\n# =================================", match.index);
  if (separator < 0) throw new Error(`Brak separatora po funkcji Python ${name}`);
  return source.slice(0, match.index) + replacement.trimEnd() + "\n\n" + source.slice(separator + 1);
}

function replaceTsFunction(source, name, replacement) {
  const match = new RegExp(`function\\s+${name}\\s*\\(`).exec(source);
  if (!match) throw new Error(`Brak funkcji TypeScript ${name}`);
  const open = source.indexOf("{", match.index);
  let depth = 0;
  for (let index = open; index < source.length; index++) {
    if (source[index] === "{") depth++;
    if (source[index] === "}") depth--;
    if (depth === 0) return source.slice(0, match.index) + replacement.trimEnd() + source.slice(index + 1);
  }
  throw new Error(`Nie domknieto funkcji TypeScript ${name}`);
}

const pythonStart = `def skrzynia_wyspy():

    # TU PRACUJE UCZEŃ:
    # oblicz pozycje, postaw skrzynie i umiesc w niej trzy nagrody.
    pass`;

const tsStart = `function skrzynia_wyspy() {
    // TU PRACUJE UCZEŃ:
    // oblicz pozycje, postaw skrzynie i umiesc w niej trzy nagrody.
}`;

const finalPath = resolve(option("--final", "../../docs/showcase/projekty/09-Podniebna-Baza-FINAL.mkcd"));
const startPath = resolve(option("--output", "../../docs/showcase/projekty/09-Podniebna-Baza-START.mkcd"));
if (!existsSync(finalPath)) throw new Error(`Brak FINAL: ${finalPath}`);

const packed = JSON.parse(await decompress(readFileSync(finalPath)));
const files = JSON.parse(packed.source);
files["main.py"] = replacePythonFunction(files["main.py"], "skrzynia_wyspy", pythonStart);
files["main.ts"] = replaceTsFunction(files["main.ts"], "skrzynia_wyspy", tsStart);

const config = JSON.parse(files["pxt.json"]);
config.name = "Podniebna Baza START";
files["pxt.json"] = JSON.stringify(config, null, 4);
packed.meta.name = "Podniebna Baza START";
packed.source = JSON.stringify(files);

writeFileSync(startPath, await compress(JSON.stringify(packed, null, 2)));
console.log(`Utworzono ${startPath}`);
