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
