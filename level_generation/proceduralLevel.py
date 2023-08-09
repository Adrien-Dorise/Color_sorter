

if __name__ == "__main__":
    #( numberTube,  numberEmptyTube,  numberInitLayers,  numberMaxLayers,  tubeToWin)
    
    #Create a single level
    """
    TOTALCOLORAVAILABLE = 8
    numberTube = 3
    numberEmptyTube = 1
    numberInitLayers = 3
    numberMaxLayers = 3
    tubesToWin = 2
    print(f"\nsetupObject.initLevelParameters({numberTube}, {numberEmptyTube}, {numberInitLayers}, {numberMaxLayers}, {tubesToWin});")
    """

    #Use this to create full layered tubes
    """
    case = 36
    n_emptyTube = 1
    for n_layers in range(4,8+1):
        for n_tube in range(5,12+1):
            print(f"\ncase {case}:\n    numberTube = {n_tube};\n    numberEmptyTube = {n_emptyTube};\n    numberInitLayers = {n_layers};\n    numberMaxLayers = {n_layers};\n    tubeToWin = {n_tube-n_emptyTube};\n    maxLevelColor = {n_tube-n_emptyTube};\n    generatorVersion= {generatorVersion};\n      seed = 0;\n\n    setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);\n    generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);\n    return generatedLevel;")
            case+=1
    """
    
    #Use this to create partially full tubes
    case = 127
    n_emptyTube = 0
    n_max_layers = 11
    generatorVersion = 1 
    for n_layers in range(4,n_max_layers):
        for n_tube in range(5,12+1):
            tubes_to_win = (n_layers*(n_tube-n_emptyTube)) / n_max_layers
            max_level_color = tubes_to_win
            if(tubes_to_win % 2 == 0):
                print(f"\ncase {case}:\n    numberTube = {n_tube};\n    numberEmptyTube = {n_emptyTube};\n    numberInitLayers = {n_layers};\n    numberMaxLayers = {n_max_layers};\n    tubeToWin = {int(tubes_to_win)};\n    maxLevelColor = {int(max_level_color)};\n    generatorVersion= {generatorVersion};\n     seed = 0;\n\n    setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);\n    generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);\n    return generatedLevel;")
                case+=1