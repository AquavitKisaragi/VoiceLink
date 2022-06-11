[System.Console]::OutputEncoding = [System.Text.Encoding]::UTF8
[String]$arg1=$args[0]
$Location = Convert-Path .
[String]$jsonfile  = $Location + "\\VoiceData\\" +$arg1 + ".json"
[String]$writeFile = $Location + "\\VoiceData\\" +$arg1 + ".wav"

if(Test-Path $jsonfile){}else{ exit -1}

$jsonContent = (Get-Content $jsonfile  -Encoding utf8 | ConvertFrom-Json)



foreach( $key in $jsonContent.psobject.properties.name) {
    switch ( $key ) {
        'text'          { $text =  $jsonContent.text    }
        'talk'          { [bool]$talk = $jsonContent.talk    }
        'speaker'       { $speaker = $jsonContent.speaker}
    }
}

try
{

$postUri = "http://localhost:50021/audio_query?text=$text&speaker=$speaker"
$responce = Invoke-RestMethod -Method Post -Uri $postUri -ContentType "application/json" -TimeoutSec 30

foreach( $key in $jsonContent.psobject.properties.name) {
    switch ( $key ) {
        'speedScale'        { $responce.speedScale = $jsonContent.speedScale   }
        'pitchScale'        { $responce.pitchScale = $jsonContent.pitchScale   }
        'intonationScale'   {  $responce.intonationScale = $jsonContent.intonationScale    }
        'volumeScale'       {  $responce.volumeScale = $jsonContent.volumeScale    }
        'prePhonemeLength'  {  $responce.prePhonemeLength = $jsonContent.prePhonemeLength    }
        'postPhonemeLength' {  $responce.postPhonemeLength = $jsonContent.postPhonemeLength    }
        'outputSamplingRate'{  $responce.outputSamplingRate = $jsonContent.outputSamplingRate    }
        'outputStereo'      {  $responce.outputStereo = $jsonContent.outputStereo    }
    }
}

$param = ConvertTo-Json -Compress $responce  -Depth 10

Invoke-RestMethod -Method POST -Uri "http://localhost:50021/synthesis?speaker=$speaker" -ContentType "application/json" -Body $param -OutFile $writeFile -TimeoutSec 30

}catch
{
    exit -2
}

if (  $talk  ){
    $player = New-Object Media.SoundPlayer $writeFile
    $player.Play()
    $player.Playsync()
}
exit 0

# SIG # Begin signature block
# MIIFbQYJKoZIhvcNAQcCoIIFXjCCBVoCAQExCzAJBgUrDgMCGgUAMGkGCisGAQQB
# gjcCAQSgWzBZMDQGCisGAQQBgjcCAR4wJgIDAQAABBAfzDtgWUsITrck0sYpfvNR
# AgEAAgEAAgEAAgEAAgEAMCEwCQYFKw4DAhoFAAQUfEZUnrsjCk6B0Rq8Bs1jIP5I
# eTugggMIMIIDBDCCAeygAwIBAgIQIRfkPcWfA6JJgZFl0xnlyzANBgkqhkiG9w0B
# AQsFADAaMRgwFgYDVQQDDA9BcXVhdml0S2lzYXJhZ2kwHhcNMjIwNjEwMDcwMTUz
# WhcNMjMxMjMwMTUwMDAwWjAaMRgwFgYDVQQDDA9BcXVhdml0S2lzYXJhZ2kwggEi
# MA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDOQXtgCfm8POUq/mwzPqBvyED2
# 6K5sFWIoY7g935cGzwOCytHOF3Z5LcBlTMpaw/CG81ZfTk5jr4s1Jon9W35MumSI
# ZOtLIzV0YwT6CO7SjdcLz/9HU+kSzIux9SF+8AP5QAIPlZRs2OKJk+fyU5lfBC/U
# VWiSGv/gAEepCUHAYqVWc5snmHV+ffmL+NJAn6vCGOkWndqoR+q8xTvKPX9Thp/C
# Njf7I4dhIkxj7I1B9FTimd37oQw/YgVmKNb8zL6CAvWO4UhQoqOpfraPEaYYCMDa
# /fZox8HfIGDdukN7G+JO4RIWnkkcTpvuG13Ni8XnbMLuVNJUkU8SS8EB0lu9AgMB
# AAGjRjBEMA4GA1UdDwEB/wQEAwIHgDATBgNVHSUEDDAKBggrBgEFBQcDAzAdBgNV
# HQ4EFgQUXu0L29Q1BIA1BJu0bqM/8kObHzUwDQYJKoZIhvcNAQELBQADggEBADsM
# 3VVajkIYdetjQ5MrU904SLi5efzufU6pIi06FnSuxPInj3Pvfv6OELwE1f7UF5Ll
# XLp5r/IBmEuB+XAzlKOaN0f8o/HS7ZDSPoKkmKN396Cj586P8qh0jPA3H9ww/OnK
# Tla7arucbQdCl7bpvX9LgLuGFJMyqzOlaMM3hW3dWloT7cytJMG1qRlTT+vDC/yf
# u5+pqXzFLiZJPpvLOv9ovcoh6Y2FfimaaBAmBoKydhBe1EJYlrnjMomNfwhZOo9C
# Iby3dVi1eaB+VnwEDpkliGIiZwsWiTy3AskStiVVnaNcWQYPduJaiCNzTBr/Td0z
# +XMZ2sbmT8Ye5FXWOT8xggHPMIIBywIBATAuMBoxGDAWBgNVBAMMD0FxdWF2aXRL
# aXNhcmFnaQIQIRfkPcWfA6JJgZFl0xnlyzAJBgUrDgMCGgUAoHgwGAYKKwYBBAGC
# NwIBDDEKMAigAoAAoQKAADAZBgkqhkiG9w0BCQMxDAYKKwYBBAGCNwIBBDAcBgor
# BgEEAYI3AgELMQ4wDAYKKwYBBAGCNwIBFTAjBgkqhkiG9w0BCQQxFgQUZpzOelj0
# izRfMEoFEG+9GmfMogEwDQYJKoZIhvcNAQEBBQAEggEAEnZyd7KxlVy5cGTxPgmH
# bOWTLWGzzT8bImicpWaOds+sMLmCAPwbbKNXnmuaMc/Kqi+T6DQR6O1OAbSaR2Cj
# 53L8sxui01xJqAqE5czN+itmpdS5bvyruZchpo3wMhnOwfQ+aybTxE/l955iHBAD
# PPO2z8VaPzXVzbwTDyT3baXVNRQrWsiV9+DKBZr2uLKvAB3iAF9hNeMyc+iHuj0d
# WSc7g5ujgLy49jjAWcW+LglBkwVlgY6T1mLMpuAMj5OGFeQllACtSqpRXoGaFLaf
# YHdeNwAdjbLY/oXcG+4OzderLMxZCGPocvI+qbk/ZPUmCxxdf/8H4tBzMlYU0FMS
# jA==
# SIG # End signature block
