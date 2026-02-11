const fs = require("fs");
const zlib = require("zlib");
const path = require("path");

const dir = path.join(__dirname, "public", "unity", "Build");
const files = [
  ["build.data.br", "build.data"],
  ["build.framework.js.br", "build.framework.js"],
  ["build.wasm.br", "build.wasm"],
];

files.forEach(([br, out]) => {
  const brPath = path.join(dir, br);
  const outPath = path.join(dir, out);
  if (fs.existsSync(brPath)) {
    fs.writeFileSync(
      outPath,
      zlib.brotliDecompressSync(fs.readFileSync(brPath)),
    );
    console.log("Decompressed: " + out);
  } else {
    console.log("Skipped (not found): " + br);
  }
});

console.log("Done!");
