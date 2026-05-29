@echo off
SETLOCAL EnableDelayedExpansion

:: --- CONFIGURATION ---
set "AWS_DEFAULT_REGION=ap-south-1"
set "AWS_PROFILE=default"
set "ECR_REGISTRY=248732772276.dkr.ecr.ap-south-1.amazonaws.com"
set "ECR_REPOSITORY=city-project"
set "IMAGE_TAG=latest"
:: ---------------------

echo.
echo [1/5] Authenticating Docker with AWS ECR (Profile: %AWS_PROFILE%)...
aws ecr get-login-password --region %AWS_DEFAULT_REGION% --profile %AWS_PROFILE% | docker login --username AWS --password-stdin %ECR_REGISTRY%
if %errorlevel% neq 0 (
    echo [ERROR] AWS Authentication failed.
    exit /b %errorlevel%
)

echo.
echo [2/5] Building and Tagging CityApi...
docker build --provenance=false -t city-api ./CityApi
docker tag city-api:latest %ECR_REGISTRY%/%ECR_REPOSITORY%:api-%IMAGE_TAG%
if %errorlevel% neq 0 (
    echo [ERROR] CityApi build failed.
    exit /b %errorlevel%
)

echo.
echo [3/5] Pushing CityApi (tag: api-%IMAGE_TAG%) to ECR...
docker push %ECR_REGISTRY%/%ECR_REPOSITORY%:api-%IMAGE_TAG%
if %errorlevel% neq 0 (
    echo [ERROR] CityApi push failed. Ensure the ECR repository '%ECR_REPOSITORY%' exists.
    exit /b %errorlevel%
)

echo.
echo [4/5] Building and Tagging CityWeb...
docker build --provenance=false -t city-web ./CityWeb
docker tag city-web:latest %ECR_REGISTRY%/%ECR_REPOSITORY%:web-%IMAGE_TAG%

if %errorlevel% neq 0 (
    echo [ERROR] CityWeb build failed.
    exit /b %errorlevel%
)

echo.
echo [5/5] Pushing CityWeb (tag: web-%IMAGE_TAG%) to ECR...
docker push %ECR_REGISTRY%/%ECR_REPOSITORY%:web-%IMAGE_TAG%
if %errorlevel% neq 0 (
    echo [ERROR] CityWeb push failed.
    exit /b %errorlevel%
)

echo.
echo ===================================================
echo [SUCCESS] Both images pushed to the SAME repository.
echo Repository: %ECR_REGISTRY%/%ECR_REPOSITORY%
echo Tags: api-%IMAGE_TAG%, web-%IMAGE_TAG%
echo ===================================================
pause
