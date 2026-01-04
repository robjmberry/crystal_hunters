import os
import math
import json
from PIL import Image

# Name of the config file to look for
CONFIG_FILE = "spritesheet_config.json"
# Name of the master readme file
README_FILE = "SPRITES_README.md"

def create_atlas_from_subfolders(job_name, root_dir, output_file, scale_factor=1.0, columns=0):
    
    print(f"\n=== Processing Job: {job_name} ===")

    if not os.path.exists(root_dir):
        print(f"  [Error] Input folder not found: {root_dir}")
        return None

    output_dir = os.path.dirname(output_file)
    if output_dir and not os.path.exists(output_dir):
        os.makedirs(output_dir)

    # 1. Identify Subfolders
    subfolders = [
        d for d in os.listdir(root_dir) 
        if os.path.isdir(os.path.join(root_dir, d)) 
        and not d.startswith('.')
    ]
    subfolders.sort()

    if not subfolders:
        print(f"  [Error] No subfolders found inside {root_dir}")
        return None

    all_images = []
    frame_ranges = {} 
    max_w = 0
    max_h = 0
    total_frames_so_far = 0

    # 2. Process each folder
    for folder in subfolders:
        folder_path = os.path.join(root_dir, folder)
        valid_extensions = ('.png', '.jpg', '.jpeg', '.bmp', '.tga')
        files = [f for f in os.listdir(folder_path) if f.lower().endswith(valid_extensions)]
        files.sort()

        if not files:
            continue

        start_index = total_frames_so_far
        
        for filename in files:
            img_path = os.path.join(folder_path, filename)
            try:
                with Image.open(img_path) as img:
                    img = img.convert("RGBA")

                    if scale_factor != 1.0:
                        new_w = int(img.width * scale_factor)
                        new_h = int(img.height * scale_factor)
                        img = img.resize((new_w, new_h), Image.Resampling.LANCZOS)
                    
                    all_images.append(img)
                    max_w = max(max_w, img.width)
                    max_h = max(max_h, img.height)
            except Exception as e:
                print(f"  [Error] Loading {filename}: {e}")

        count_in_folder = len(files)
        total_frames_so_far += count_in_folder
        end_index = total_frames_so_far - 1
        frame_ranges[folder] = (start_index, end_index)

    # 3. Calculate Grid
    total_count = len(all_images)
    if total_count == 0:
        print("  [Error] No valid images found.")
        return None

    if columns > 0:
        cols = columns
        rows = math.ceil(total_count / columns)
    else:
        cols = math.ceil(math.sqrt(total_count))
        rows = math.ceil(total_count / cols)

    sheet_width = cols * max_w
    sheet_height = rows * max_h

    # 4. Generate Sheet
    sprite_sheet = Image.new("RGBA", (sheet_width, sheet_height), (0, 0, 0, 0))

    for index, img in enumerate(all_images):
        col_idx = index % cols
        row_idx = index // cols
        
        x_pos = col_idx * max_w
        y_pos = row_idx * max_h

        x_offset = (max_w - img.width) // 2
        y_offset = (max_h - img.height) // 2

        sprite_sheet.paste(img, (x_pos + x_offset, y_pos + y_offset))

    sprite_sheet.save(output_file)
    print(f"  [Success] Image saved to: {output_file}")

    # Return data needed for the master readme
    return {
        "job_name": job_name,
        "output_file": output_file,
        "sheet_w": sheet_width,
        "sheet_h": sheet_height,
        "cell_w": max_w,
        "cell_h": max_h,
        "frame_ranges": frame_ranges
    }

def generate_master_readme(all_results):
    """
    Takes a list of job results and writes one giant README file.
    """
    with open(README_FILE, 'w') as f:
        f.write("# Master Sprite Sheet Documentation\n\n")
        f.write("This file is auto-generated. It contains frame indices for all characters.\n")
        f.write("Use these values in Godot's AnimationPlayer.\n\n")
        
        for result in all_results:
            name = result['job_name']
            path = result['output_file']
            dims = f"{result['sheet_w']}x{result['sheet_h']}"
            cell = f"{result['cell_w']}x{result['cell_h']}"
            
            f.write(f"## {name}\n")
            f.write(f"- **Path:** `{path}`\n")
            f.write(f"- **Sheet Size:** {dims} px\n")
            f.write(f"- **Cell Size:** {cell} px\n\n")
            
            f.write("| Animation | Start | End | Count |\n")
            f.write("| :--- | :---: | :---: | :---: |\n")
            
            for anim, (start, end) in result['frame_ranges'].items():
                count = (end - start) + 1
                f.write(f"| {anim} | {start} | {end} | {count} |\n")
            
            f.write("\n---\n\n")
            
    print(f"\n[Done] Master README updated at: {README_FILE}")

def load_config_and_run():
    if not os.path.exists(CONFIG_FILE):
        print(f"Configuration file '{CONFIG_FILE}' not found.")
        return

    try:
        with open(CONFIG_FILE, 'r') as f:
            jobs = json.load(f)
            
        print(f"Found {len(jobs)} jobs in config.")
        
        results = []

        for job in jobs:
            name = job.get("name", "Unnamed Job")
            input_folder = job.get("input_folder")
            output_file = job.get("output_file")
            scale = job.get("scale", 1.0)
            columns = job.get("columns", 0)
            
            if input_folder and output_file:
                # Run the job and collect the result
                data = create_atlas_from_subfolders(name, input_folder, output_file, scale, columns)
                if data:
                    results.append(data)
            else:
                print(f"Skipping job '{name}': Missing input_folder or output_file")
        
        # Once all jobs are done, write the single readme
        if results:
            generate_master_readme(results)
                
    except json.JSONDecodeError:
        print(f"Error: '{CONFIG_FILE}' is not valid JSON.")

if __name__ == "__main__":
    load_config_and_run()