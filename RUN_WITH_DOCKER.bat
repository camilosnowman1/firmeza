@echo off
echo ==========================================
echo      Firmeza - Run with Docker
echo ==========================================
echo.
echo Stopping any running containers...
docker-compose -f docker-compose.yml down

echo.
echo Building and starting containers...
docker-compose -f docker-compose.yml up --build -d

echo.
echo ==========================================
echo      Application Status
echo ==========================================
echo.
echo API is running at: http://localhost:5000
echo Web Admin is running at: http://localhost:5001
echo Client is running at: http://localhost:4200
echo.
echo To stop the application, run: docker-compose down
echo.
pause
