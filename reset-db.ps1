# Reset Database Script for Hotel SAAS
Write-Host "Stopping Postgres container..." -ForegroundColor Cyan
docker-compose stop postgres

Write-Host "Removing Postgres volume..." -ForegroundColor Cyan
docker volume rm hotel-saas-backend_postgres_data

Write-Host "Starting Postgres container..." -ForegroundColor Cyan
docker-compose up -d postgres

Write-Host "Database reset complete! Now start your backend to run migrations and seed." -ForegroundColor Green
