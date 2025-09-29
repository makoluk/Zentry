# Simple HTTP Server for testing
$port = 8080
$path = Get-Location

Write-Host "Starting HTTP server on port $port"
Write-Host "Open: http://localhost:$port"
Write-Host "Press Ctrl+C to stop"

# Create a simple HTTP server
$listener = New-Object System.Net.HttpListener
$listener.Prefixes.Add("http://localhost:$port/")

try {
    $listener.Start()
    Write-Host "Server started successfully!"
    
    while ($listener.IsListening) {
        $context = $listener.GetContext()
        $request = $context.Request
        $response = $context.Response
        
        $localPath = $request.Url.LocalPath
        if ($localPath -eq "/") { $localPath = "/index.html" }
        
        $filePath = Join-Path $path $localPath.TrimStart('/')
        
        if (Test-Path $filePath) {
            $content = Get-Content $filePath -Raw -Encoding UTF8
            $buffer = [System.Text.Encoding]::UTF8.GetBytes($content)
            
            # Set content type
            if ($filePath.EndsWith('.html')) {
                $response.ContentType = "text/html; charset=utf-8"
            } elseif ($filePath.EndsWith('.js')) {
                $response.ContentType = "application/javascript; charset=utf-8"
            } elseif ($filePath.EndsWith('.css')) {
                $response.ContentType = "text/css; charset=utf-8"
            }
            
            $response.ContentLength64 = $buffer.Length
            $response.OutputStream.Write($buffer, 0, $buffer.Length)
        } else {
            $response.StatusCode = 404
            $buffer = [System.Text.Encoding]::UTF8.GetBytes("404 Not Found")
            $response.ContentLength64 = $buffer.Length
            $response.OutputStream.Write($buffer, 0, $buffer.Length)
        }
        
        $response.Close()
    }
} catch {
    Write-Host "Error: $_"
} finally {
    $listener.Stop()
}
