# COMP-7051-Assignment-2

### [45 marks] Maze Creation
Write a game in **Unity3D** that runs on **Windows** and draws a **3D maze**.  
The maze can be:
- Generated randomly or programmatically, **or**
- Loaded in as a resource.

#### Requirements:
- Maze must be **at least 5x5** in size.
- **Walls and floor** of the maze must have **textures**.
- Each of the following should have **different textures**:
  - North walls  
  - South walls  
  - East walls  
  - West walls  
  - Floor tiles  
- Each wall is **double-sided**, and may have a **different texture on each side**.
- The maze should have a **start** and **end** point.
- There must be a **path from start to end**.
- When the player reaches the end of the maze:
  - Display text indicating that the player has found the exit and **won**.

---

### [25 marks] Movement Controls
Add the ability to move through the maze using **standard first-person shooter (FPS) controls**.

#### Requirements:
- **First-person perspective**.
- **WASD**: move forward, backward, strafe left/right.  
- **Mouse**: controls camera rotation.  
- Add controls for **both keyboard and gamepad**.

---

### [5 marks] Collision Toggle
By default, the player should **not** be able to walk through walls.

#### Requirements:
- Pressing a **key on the keyboard** or a **button on the gamepad** should **toggle** this behavior.  
- Both input methods must work.

---

### [5 marks] Look Controls
Add controls that allow the player to **look up or down**.

---

### [5 marks] Reset Function
Pressing the **HOME key** (keyboard) or a **button on the gamepad** should:

- Reset the player to the **starting point** of the maze.  
- Reset the **view** to default.  
- Reset the **enemy** to their original spawn position.

---

### [15 marks] Enemy Object
Add an **enemy object** to the game.

#### Requirements:
- Use a **model** (e.g., a skinned animation model covered in class).  
- The enemy should **move independently** throughout the maze.  
- The enemy should **play a walking or running animation**.  
- The **movement AI** does **not** need to be sophisticated.

---

### Submission Requirements

- **Build your project** and provide a **runnable executable**.  
  - ⚠️ **-25% penalty** if missing.
- Include a **video** of your game running that demonstrates completed features.  
  - Narrate the video explaining completed and uncompleted features.  
  - ⚠️ **-25% penalty** if missing.

#### What to Submit:
- Your **entire project**, including:
  - A **README file** with:
    - Notes about your project
    - Description of user controls
- Submit as a **single ZIP file** using this naming convention:

