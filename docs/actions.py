## General remarks:
# A City of family type implies a Village of family type (as a village is needed before a city can grow)

## HELPER FUNCTIONS:
# Checks whether a settlement upgrade should be added to the tile in question. Returns boolean.
def checkSettlementUpgrade(family: str):
    if "Tile contains settlement not family":
        "continue/skip"
    elif "Tile contains village of family":
        if "Tile.familyScore > Max(not familyScores) + 15":
            "Remove Village, add City of family to Tile"
    else:
        if "Tile.familyScore > Max(not familyScores) + 10":
            "Add village of family to Tile."


## PLANT FUNCTIONS:
# Colonization function for plants.
def plantRuleColonization():
    checkSettlementUpgrade("Plant")

    if "Tile.AnimalScore + Tile.FungiScore > 0":
        "Tile.PlantScore += 2"
    else:
        "Tile.PlantScore += 10"


# Assimilate function for plants.
def plantRuleAssimilate():
    if "Tile.AnimalScore + Tile.FungiScore > 0":
        "Tile.PlantScore += 6"
    else:
        "Tile.PlantScore += 2"


# Rooting function for plants.
def plantRuleRooting():
    if "Tile.PlantScore > Max(Tile.AnimalScore, Tile.FungiScore)":
        "SubsurfaceTile.PlantScore += 10"


# Decay function for plants.
def plantRuleDecay():
    "Tile.AnimalScore = Tile.AnimalScore / 2"
    "Tile.FungiScore = Tile.FungiScore / 2"


# Debilitating Vines function for plants.
def plantRuleDebilitatingVines():
    "Place token on Tile, preventing first upcoming non-plant score increase."


# Rampant Growth function for plants
def plantRuleRampantGrowth():
    "value = Tile.PlantScore / 2"
    for i in "SurroundingTiles":
        "Tile.PlantScore += value"


## FUNGI FUNCTIONS:
# Colonization function for fungi.
def fungiRuleColonization():
    checkSettlementUpgrade("fungi")
    
    if "Tile.AnimalScore + Tile.PlantScore > 0":
        "Tile.FungiScore += 2"
    else:
        "Tile.FungiScore += 10"


## ANIMAL FUNCTIONS:
