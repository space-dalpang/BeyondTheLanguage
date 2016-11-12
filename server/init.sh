#!/usr/bin/env bash
set -e

docker-compose exec flask python3 /var/www/flask/manager.py db init || echo "Skip init"
docker-compose exec flask python3 /var/www/flask/manager.py db migrate
docker-compose exec flask python3 /var/www/flask/manager.py db upgrade
curl -X "POST" "http://localhost/words" \
     -H "Content-Type: application/json" \
     -d "@words.json"

