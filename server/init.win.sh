#!/usr/bin/env bash
set -e

winpty docker exec -it server_flask_1 sh -c "python3 /var/www/flask/manager.py db init || echo \"Skip init\""
winpty docker exec -it server_flask_1 sh -c "python3 /var/www/flask/manager.py db migrate"
winpty docker exec -it server_flask_1 sh -c "python3 /var/www/flask/manager.py db upgrade"
curl -X "POST" "http://192.168.99.100/words" \
     -H "Content-Type: application/json" \
     -d "@words.json"
