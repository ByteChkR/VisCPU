function Zip-Pack {

    param (
        [string]$in,
        [string]$out
     )

    Add-Type -Assembly System.IO.Compression.FileSystem
    $compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
    [System.IO.Compression.ZipFile]::CreateFromDirectory($in, $out)

}

function Zip-Unpack {

    param (
        [string]$in,
        [string]$out
     )

    Add-Type -Assembly System.IO.Compression.FileSystem
    $compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
    [System.IO.Compression.ZipFile]::ExtractToDirectory($in, $out)

}
