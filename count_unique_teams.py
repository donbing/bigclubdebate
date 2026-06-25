import csv
import os
import re

root = os.path.join(os.getcwd(), 'BigClubDebate.Web', 'GameData')
england_master = os.path.join(root, 'england-master')
seasons = sorted(
    d for d in os.listdir(england_master)
    if os.path.isdir(os.path.join(england_master, d)) and re.match(r'^\d{4}-\d{2}$', d)
)
latest = seasons[-1]
teamset = set()

print('latest season:', latest)

for fname in os.listdir(os.path.join(england_master, latest)):
    if fname.endswith('.txt') and not fname.endswith('.conf.txt') and not fname.endswith('playoffs.txt'):
        path = os.path.join(england_master, latest, fname)
        with open(path, encoding='utf-8') as f:
            for line in f:
                if line.startswith('  '):
                    parts = line.strip().split()
                    for i, token in enumerate(parts):
                        if '-' in token and all(x.isdigit() for x in token.split('-') if x):
                            home = ' '.join(parts[:i]).strip()
                            away = ' '.join(parts[i+1:-1]).strip()
                            if home:
                                teamset.add(home)
                            if away:
                                teamset.add(away)
                            break

print('league teams:', len(teamset))

for cupfile in ['facup.csv.txt', 'leaguecup.csv.txt']:
    path = os.path.join(root, cupfile)
    if os.path.exists(path):
        with open(path, encoding='utf-8') as f:
            reader = csv.reader(f)
            next(reader, None)
            for row in reader:
                if len(row) >= 4:
                    if row[2].strip():
                        teamset.add(row[2].strip())
                    if row[3].strip():
                        teamset.add(row[3].strip())

print('league + cups:', len(teamset))

for fname, cols in [('engsoccerdata/champs.csv', (4, 5)), ('transfermarkt/games.csv', (19, 20))]:
    path = os.path.join(root, fname)
    if os.path.exists(path):
        with open(path, encoding='utf-8') as f:
            reader = csv.reader(f)
            next(reader, None)
            for row in reader:
                if len(row) > cols[1]:
                    if row[cols[0]].strip():
                        teamset.add(row[cols[0]].strip())
                    if row[cols[1]].strip():
                        teamset.add(row[cols[1]].strip())

print('all sources:', len(teamset))
