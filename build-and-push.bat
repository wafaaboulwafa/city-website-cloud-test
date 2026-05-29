@echo off
SETLOCAL EnableDelayedExpansion

:: --- CONFIGURATION ---
SET AWS_REGION=us-east-1
SET AWS_ACCOUNT_ID=YOUR_ACCOUNT_ID
SET ECR_REPO_API=city-api
SET ECR_REPO_WEB=city-web
SET IMAGE_TAG=latest
:: ---------------------

SET ECR_REGISTRY=%AWS_ACCOUNT_ID%.dkr.ecr.%AWS_REGION%.amazonaws.com

echo.
echo [1/5] Authenticating Docker with AWS ECR...
aws ecr get-login-password --region %AWS_REGION% | docker login --username AWS --password-stdin %ECR_REGISTRY%
if %errorlevel% neq 0 (
    echo [ERROR] AWS Authentication failed. Ensure AWS CLI is configured correctly.
    exit /b %errorlevel%
)

echo.
echo [2/5] Building and Tagging CityApi...
docker build -t %ECR_REPO_API% ./CityApi
docker tag %ECR_REPO_API%:%IMAGE_TAG% %ECR_REGISTRY%/%ECR_REPO_API%:%IMAGE_TAG%
if %errorlevel% neq 0 (
    echo [ERROR] CityApi build failed.
    exit /b %errorlevel%
)

echo.
echo [3/5] Pushing CityApi to ECR...
docker push %ECR_REGISTRY%/%ECR_REPO_API%:%IMAGE_TAG%
if %errorlevel% neq 0 (
    echo [ERROR] CityApi push failed. Ensure the ECR repository '%ECR_REPO_API%' exists.
    exit /b %errorlevel%
)

echo.
echo [4/5] Building and Tagging CityWeb...
docker build -t %ECR_REPO_WEB% ./CityWeb
docker tag %ECR_REPO_WEB%:%IMAGE_TAG% %ECR_REGISTRY%/%ECR_REPO_WEB%:%IMAGE_TAG%
if %errorlevel% neq 0 (
    echo [ERROR] CityWeb build failed.
    exit /b %errorlevel%
)

echo.
echo [5/5] Pushing CityWeb to ECR...
docker push %ECR_REGISTRY%/%ECR_REPO_WEB%:%IMAGE_TAG%
if %errorlevel% neq 0 (
    echo [ERROR] CityWeb push failed. Ensure the ECR repository '%ECR_REPO_WEB%' exists.
    exit /b %errorlevel%
)

echo.
echo ===================================================
echo [SUCCESS] All images built and pushed to AWS ECR.
echo Registry: %ECR_REGISTRY%
echo ===================================================
pause
