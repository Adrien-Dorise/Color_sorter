

if __name__ == "__main__":
    #( numberTube,  numberEmptyTube,  numberInitLayers,  numberMaxLayers,  tubeToWin)

    TOTALCOLORAVAILABLE = 8

    numberTube = 3
    numberEmptyTube = 1
    numberInitLayers = 3
    numberMaxLayers = 3
    tubesToWin = 2
    #print(f"\nsetupObject.initLevelParameters({numberTube}, {numberEmptyTube}, {numberInitLayers}, {numberMaxLayers}, {tubesToWin});")


#    for i in range(70,80):
#        print(f"\ncase {i}:\n    numberTube = 5;\n    numberEmptyTube = 1;\n    numberInitLayers = 4;\n    numberMaxLayers = 4;\n    tubeToWin = 4;\n    maxLevelColor = 4;\n    seed = 0;\n\n    setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);\n    generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor);\n    return generatedLevel;")
    
    case = 36
    n_emptyTube = 1
    for n_layers in range(4,8+1):
        for n_tube in range(5,12+1):
            print(f"\ncase {case}:\n    numberTube = {n_tube};\n    numberEmptyTube = {n_emptyTube};\n    numberInitLayers = {n_layers};\n    numberMaxLayers = {n_layers};\n    tubeToWin = {n_tube-n_emptyTube};\n    maxLevelColor = {n_tube-n_emptyTube};\n    seed = 1;\n\n    setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);\n    generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor);\n    return generatedLevel;")
            case+=1