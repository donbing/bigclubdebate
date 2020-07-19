
# the big club debate

use ojective (ish) stastitics to determine which of two football clubs is bigger..

## features

 - list list, competition stats, game stats and goal stats for each competition
 - select two teams from title drop-down list
 - expandable stats
 - team logos
 - custom background per matchup
 - tooltip extra-stats
 
## data

 - taken from: https://github.com/openfootball/england
 - FACup, League Cup and Division 1-4 stats have been gathered and used to project interesting statistic about two chosen clubs
 - stadium info is available for staduim coordinates / google maps display)
 - I've not found any good player data, maybe if i do, more interesting stats can be added.

## hacks
 - msbuild csproj hack: game data is copied from external folder with in [Web.csproj](the website)

## todo
front:
 
 - last update date at top
 - custom tagline for derbys
 - nicer background transition
 - nicer fonts

 - add stadium info
 - add more .extras (tool tip items)
 
 - add player/graph/transfers dialog
 - make more/less transion even smoother
 - make more/less button take up less space

 * fix numbers not fitting in boxes
 * custom backgrounds per team
 * resize logos

 back:
  - how to store data
  - how to keep data up to date
