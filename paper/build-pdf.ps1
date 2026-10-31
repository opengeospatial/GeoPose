<#
.SYNOPSIS
    Converts a Markdown file to PDF with images, LaTeX math and Mermaid diagrams.

.DESCRIPTION
    Pandoc converts the Markdown to a standalone HTML page in a temp folder, and
    headless Microsoft Edge prints that page to PDF. A <base> tag points relative
    image paths (e.g. images/ecef.png) back at the Markdown file's folder.
    MathJax and Mermaid load from cdn.jsdelivr.net, so an internet connection is needed.

.EXAMPLE
    .\build-pdf.ps1
    .\build-pdf.ps1 -InputFile .\WGS84_G2296_Geodetic_Reference_System.md
#>
param(
    [string]$InputFile = (Join-Path $PSScriptRoot 'GeoPose_Research_Paper.md'),
    [string]$OutputFile
)

$ErrorActionPreference = 'Stop'

$md = (Resolve-Path $InputFile).Path
$dir = Split-Path $md -Parent
$name = [IO.Path]::GetFileNameWithoutExtension($md)
if (-not $OutputFile) { $OutputFile = Join-Path $dir "$name.pdf" }
$OutputFile = [IO.Path]::GetFullPath($OutputFile)

# Locate tools
$pandoc = (Get-Command pandoc -ErrorAction SilentlyContinue).Source
if (-not $pandoc) { throw 'pandoc not found on PATH.' }
$edge = @(
    "${env:ProgramFiles(x86)}\Microsoft\Edge\Application\msedge.exe",
    "$env:ProgramFiles\Microsoft\Edge\Application\msedge.exe"
) | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $edge) { throw 'Microsoft Edge not found.' }

# Check image references before building
$text = [IO.File]::ReadAllText($md)
$lines = $text -split "`n"
for ($i = 0; $i -lt $lines.Count; $i++) {
    if ($lines[$i] -match '!\[[^\]]*\]\([^)]*$') {
        Write-Warning "Line $($i + 1): image link is missing its closing ')'"
    }
}
$refs = [regex]::Matches($text, '!\[[^\]]*\]\(<?([^)\s>]+)|<img[^>]*\ssrc="([^"]+)"') |
    ForEach-Object { if ($_.Groups[1].Success) { $_.Groups[1].Value } else { $_.Groups[2].Value } }
foreach ($ref in $refs) {
    if ($ref -match '^(data:|https?:)') { continue }
    $path = Join-Path $dir ([Uri]::UnescapeDataString($ref))
    if (-not (Test-Path $path)) { Write-Warning "Missing image: $ref" }
}

# HTML header: base path for images, print styles, Mermaid rendering
$base = ([Uri]($dir.TrimEnd('\') + '\')).AbsoluteUri
$head = @"
<base href="$base">
<style>
body{font-family:"Segoe UI",Arial,sans-serif;font-size:11pt;line-height:1.45;max-width:none;margin:0;padding:0}
img{max-width:100%}
table{border-collapse:collapse;margin:1em 0;font-size:10pt}
th,td{border:1px solid #999;padding:4px 6px;vertical-align:top}
th{background:#eee}
pre{white-space:pre-wrap;background:#f6f6f6;padding:6px;font-size:9pt}
pre.mermaid{background:none;text-align:center}
h1,h2,h3{page-break-after:avoid}
pre,table,img,svg{page-break-inside:avoid}
@page{size:A4;margin:20mm 18mm}
</style>
<script type="module">
import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@10.9.1/dist/mermaid.esm.min.mjs';
mermaid.initialize({startOnLoad:false,theme:'default'});
document.querySelectorAll('pre.mermaid').forEach(e=>{e.textContent=e.textContent;});
await mermaid.run({querySelector:'pre.mermaid'});
</script>
"@

$tmp = Join-Path $env:TEMP "build-pdf-$name"
New-Item -ItemType Directory -Force $tmp | Out-Null
$headFile = Join-Path $tmp 'head.html'
$htmlFile = Join-Path $tmp "$name.html"
[IO.File]::WriteAllText($headFile, $head, (New-Object Text.UTF8Encoding $false))

# Pandoc and Edge write harmless warnings to stderr, which Windows PowerShell
# would otherwise treat as fatal errors.
$ErrorActionPreference = 'Continue'

Write-Host "Converting $md ..."
& $pandoc $md -f gfm+tex_math_dollars -t html5 -s `
    --mathjax=https://cdn.jsdelivr.net/npm/mathjax@3.2.2/es5/tex-chtml-full.js `
    --metadata "pagetitle=$name" -H $headFile -o $htmlFile 2>$null
if ($LASTEXITCODE -ne 0) { throw "pandoc failed with exit code $LASTEXITCODE" }

Write-Host "Printing PDF ..."
$uri = ([Uri]$htmlFile).AbsoluteUri
& $edge --headless=new --disable-gpu --allow-file-access-from-files --no-pdf-header-footer `
    --virtual-time-budget=30000 "--print-to-pdf=$OutputFile" $uri 2>$null | Out-Null
$ErrorActionPreference = 'Stop'

if (-not (Test-Path $OutputFile)) { throw 'PDF was not created.' }
$pdf = Get-Item $OutputFile
$images = (Select-String -Path $OutputFile -Pattern '/Subtype\s*/Image' -AllMatches).Matches.Count
Write-Host ("Wrote {0} ({1:N0} KB, {2} images, {3} image references in Markdown)" -f `
    $pdf.FullName, ($pdf.Length / 1KB), $images, @($refs).Count)
