

def load_level_from_file(file_path):
    file_level_setup = ""
    try:
        with open(file_path, 'r') as file:
            for line in file:
                file_level_setup += line
    except FileNotFoundError:
        print(f"File not found: {file_path}")
    except Exception as e:
        print(f"An error occurred: {e}")
    return file_level_setup
    

def reorder_levels(original_levels, correct_order):
    new_level_setup = ""
    idx = 0
    for line in original_levels.split("\n"):
        for word in line.split():        
            if(word == "case"):
                line = f"           case {correct_order[idx]}:"
                idx+=1
                break
        new_level_setup += line + "\n"
    return new_level_setup


def reorder_in_ascending(original_levels):
    ordered_level_setup = ["" for _ in range(203)]
    ordered_level = ""
    idx = 0
    for line in original_levels.split("\n"):
        for word in line.split():
            if(word == "case"):
                idx = int(line.split()[1][0:-1])
        ordered_level_setup[idx] += line + "\n"
    
    for case in ordered_level_setup:
        ordered_level += case
    return ordered_level


def duplicate_levels(original_levels, levels_to_duplicate, number_of_levels):
    duplicated_levels_setup = ["" for _ in range(number_levels)]
    duplicated_levels = ""
    duplicated_idx = 0
    case_idx = 0
    isToDuplicate = False
    for line in original_levels.split("\n"):
        for word in line.split():
            if(word == "case"):
                if(isToDuplicate):
                    current_duplicate_idx = levels_to_duplicate.index(case_idx)
                    while(levels_to_duplicate[current_duplicate_idx] == case_idx):
                        duplicated_levels_setup[duplicated_idx+1] = duplicated_levels_setup[duplicated_idx]
                        duplicated_idx+=1
                        current_duplicate_idx+=1
                        if(current_duplicate_idx >= len(levels_to_duplicate)):
                            break

                duplicated_idx+=1
                case_idx+=1
                
                if int(line.split()[1][0:-1]) in levels_to_duplicate:
                    isToDuplicate = True
                else:
                    isToDuplicate = False
                    
        duplicated_levels_setup[duplicated_idx] += line + "\n"
    for case in duplicated_levels_setup:
        duplicated_levels += case
    return duplicated_levels
    
    
def set_case_numbers(original_levels):
    ordered_level_setup = ["" for _ in range(300)]
    ordered_level = ""
    idx = 0
    for line in original_levels.split("\n"):
        for word in line.split():
            if(word == "case"):
                is_case_line = True
                idx +=1
                break
            else:
                is_case_line = False
        if not is_case_line:
            ordered_level_setup[idx] += line + "\n"
    
    idx = 1
    for case in ordered_level_setup:
        if(case != ""):
            ordered_level += f"           case {idx}:\n"
            ordered_level += case
            idx+=1
    return ordered_level


if __name__ == "__main__":
    
    cases = ["Duplicate levels", "reorder"]
    current_case = cases[1]
    
    print("Level setup script")

    #Read the levels
    file_path = 'level_state_v1.1.3.txt'
    file_level_setup = load_level_from_file(file_path)

    if(current_case == "Duplicate levels"):  
        #Here we set the correct case number for each levels
        correct_order = [12, 28, 42, 57, 74, 90, 108, 13, 30, 43, 58, 75, 91, 110, 14, 31, 47, 61, 78, 95, 112, 15, 29, 45, 59, 81, 101, 118, 16, 33, 48, 67, 86, 104, 122, 4, 20, 34, 46, 60, 76, 92, 125, 5, 21, 35, 49, 63, 80, 96, 126, 8, 22, 37, 50, 69, 84, 98, 127, 9, 23, 38, 53, 72, 85, 102, 128, 10, 25, 39, 52, 68, 83, 99, 129, 1, 18, 3, 56, 32, 79, 51, 55, 6, 62, 71, 77, 36, 82, 88, 17, 93, 2, 64, 100, 26, 105, 111, 117, 119, 124, 44, 109, 24, 65, 113, 120, 40, 41, 54, 114, 87, 19, 107, 66, 27, 73, 121, 70, 7, 94, 115, 116, 123, 11, 89, 97, 103, 106, 130]
        original_levels = file_level_setup
        new_level_setup = reorder_levels(original_levels, correct_order)
        #print(new_level_setup)
        

        #Here we re-order the case in ascending order
        original_levels = new_level_setup
        ordered_level = reorder_in_ascending(original_levels)
        print(ordered_level)
        
        
        # Duplicate some levels, and reinsert them in the setup
        levels_to_duplicate = [23,24,25,26,27,36,37,38,39,40,41,48,49,50,51,52,53,54,55,61,62,63,64,65,65,66,66,67,67,68,68,69,69,70,70,71,72,73,76,77,78,79,79,80,80,81,81,82,82,83,83,84,84,85,86,93,94,95,96,97,98,99,100,101,102,103,104,109,110,111,112,113,114,115,116,117]
        number_levels = 250
        original_levels = ordered_level
        duplicated_levels = duplicate_levels(original_levels,levels_to_duplicate,number_levels)
        duplicated_levels = set_case_numbers(duplicated_levels)
        #print(duplicated_levels)
    
    elif(current_case == "reorder"):
        ordered_level = set_case_numbers(file_level_setup)
        print(ordered_level)
    
        