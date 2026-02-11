# update-game.ps1
# สคริปต์สำหรับอัพเดท Unity WebGL build ไปยังเว็บ
# วิธีใช้: .\update-game.ps1

$source = ".\NurseProject\build"
$dest   = ".\public\unity"

Write-Host "=== Updating Unity WebGL Build ===" -ForegroundColor Cyan

# 1. ลบ Build เก่า
if (Test-Path "$dest\Build") {
    Remove-Item -Recurse -Force "$dest\Build"
    Write-Host "[1/4] Removed old Build folder" -ForegroundColor Yellow
}

# 2. คัดลอก Build ใหม่
Copy-Item -Recurse "$source\Build" "$dest\Build"
Write-Host "[2/4] Copied new Build folder" -ForegroundColor Green

# 3. คัดลอก TemplateData ใหม่
if (Test-Path "$dest\TemplateData") {
    Remove-Item -Recurse -Force "$dest\TemplateData"
}
Copy-Item -Recurse "$source\TemplateData" "$dest\TemplateData"
Write-Host "[3/4] Copied new TemplateData folder" -ForegroundColor Green

# 4. Decompress .br files
Write-Host "[4/4] Decompressing Brotli files..." -ForegroundColor Yellow

$buildDir = (Resolve-Path "$dest\Build").Path -replace '\\','/'

node -e "var z=require('zlib'),f=require('fs'),d='$buildDir/';var pairs=[['build.data.br','build.data'],['build.framework.js.br','build.framework.js'],['build.wasm.br','build.wasm']];pairs.forEach(function(p){try{f.writeFileSync(d+p[1],z.brotliDecompressSync(f.readFileSync(d+p[0])));console.log('  Decompressed: '+p[1])}catch(e){console.log('  Skipped: '+p[0]+' ('+e.message+')')}})"

Write-Host ""
Write-Host "=== Done! Refresh browser to see changes ===" -ForegroundColor Cyan
