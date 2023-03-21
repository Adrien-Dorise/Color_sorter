

if __name__ == "__main__":
    #( numberTube,  numberEmptyTube,  numberInitLayers,  numberMaxLayers,  tubeToWin)

    TOTALCOLORAVAILABLE = 8

    numberTube = 3
    numberEmptyTube = 1
    numberInitLayers = 3
    numberMaxLayers = 3
    tubesToWin = 2
    #print(f"\nsetupObject.initLevelParameters({numberTube}, {numberEmptyTube}, {numberInitLayers}, {numberMaxLayers}, {tubesToWin});")


    for i in range(70,80):
        print(f"\ncase {i}:\n    setupObject.initLevelParameters(5, 1, 4, 4, 4);\n    numberTube = 5;\n    numberEmptyTube = 1;\n    numberInitLayers = 4;\n    numberMaxLayers = 4;\n    tubeToWin = 4;\n    maxLevelColor = 4;\n    seed = 0;\n\n    setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);\n    generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor);\n    return generatedLevel;")
