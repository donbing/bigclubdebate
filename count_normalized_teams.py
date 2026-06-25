import csv
import os
import re

root = os.path.join(os.getcwd(), 'BigClubDebate.Web', 'GameData')
england_master = os.path.join(root, 'england-master')
seasons = sorted(d for d in os.listdir(england_master) if os.path.isdir(os.path.join(england_master, d)) and re.match(r'^\d{4}-\d{2}$', d))
latest = seasons[-1]


def normalize_champs(name: str) -> str:
    name = name.strip()
    if name.lower().endswith(' football club'):
        name = name[:-len(' Football Club')]
    if name.lower().endswith(' fc'):
        name = name[:-len(' FC')]
    return name.rstrip('.')


def normalize_transfermarkt(name: str) -> str:
    name = name.strip()
    if name.lower().endswith(' football club'):
        name = name[:-len(' Football Club')]
    elif name.lower().endswith(' futbol club'):
        name = name[:-len(' Futbol Club')]
    elif name.lower().endswith(' futebol clube'):
        name = name[:-len(' Futebol Clube')]
    if name.lower().startswith('club '):
        name = name[5:].strip()
    if name.lower().endswith(' s.a.d.'):
        name = name[:-len(' S.A.D.')]
    if name.lower().endswith(' sad'):
        name = name[:-len(' SAD')]
    if name.lower().endswith(' club de fútbol'):
        name = name[:-len(' Club de Fútbol')]
    if name.lower().endswith(' club de futbol'):
        name = name[:-len(' Club de Futbol')]
    if name.lower() == 'atlético de madrid':
        name = 'Atletico Madrid'
    if name.lower().startswith('fc '):
        name = name[3:].strip()
    name = name.replace('München', 'Munich').replace('Köln', 'Koln').replace('Nürnberg', 'Nurnberg')
    if name.lower().endswith(' fc'):
        name = name[:-len(' FC')]
    if name.lower().startswith('afc '):
        name = name[4:].strip()
    return name.rstrip('.')


def normalized_team_name(name: str, source: str) -> str:
    if source == 'champs':
        return normalize_champs(name)
    return normalize_transfermarkt(name)


def add_team(teamset, team):
    if team and team.strip():
        teamset.add(team.strip())

league = set()
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
                            add_team(league, home)
                            add_team(league, away)
                            break

raw = set(league)
all_norm = set(league)

for cupfile in ['facup.csv.txt', 'leaguecup.csv.txt']:
    path = os.path.join(root, cupfile)
    if os.path.exists(path):
        with open(path, encoding='utf-8') as f:
            reader = csv.reader(f)
            next(reader, None)
            for row in reader:
                if len(row) >= 4:
                    add_team(raw, row[2])
                    add_team(raw, row[3])
                    add_team(all_norm, row[2])
                    add_team(all_norm, row[3])

for fname, cols in [('engsoccerdata/champs.csv', (4, 5)), ('transfermarkt/games.csv', (19, 20))]:
    path = os.path.join(root, fname)
    if os.path.exists(path):
        with open(path, encoding='utf-8') as f:
            reader = csv.reader(f)
            next(reader, None)
            for row in reader:
                if len(row) > cols[1]:
                    add_team(raw, row[cols[0]])
                    add_team(raw, row[cols[1]])
                    all_norm.add(normalized_team_name(row[cols[0]], 'transfermarkt' if fname.startswith('transfermarkt') else 'champs'))
                    all_norm.add(normalized_team_name(row[cols[1]], 'transfermarkt' if fname.startswith('transfermarkt') else 'champs'))

print('latest season', latest)
print('raw team count', len(raw))
print('normalized count', len(all_norm))
