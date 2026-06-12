#!/bin/bash

# Solar Defender - Unity Launcher
# Usage: ./run_game.sh

PROJECT_PATH="/home/developer/Documents/SolarDefender"

echo "🚀 Starting Solar Defender..."

# Find Unity executable
if command -v unity &> /dev/null; then
    UNITY_CMD="unity"
elif [ -d "/usr/share/unity-editor" ]; then
    UNITY_CMD="/usr/share/unity-editor/Editor/Unity"
elif [ -d "$HOME/Unity/Hub/Editor" ]; then
    UNITY_CMD="$HOME/Unity/Hub/Editor/2021.3/Editor/Unity"
else
    echo "❌ Unity not found!"
    echo "Please install Unity Hub and Unity 2021.3+"
    exit 1
fi

# Launch Unity in play mode
echo "📂 Project: $PROJECT_PATH"
$UNITY_CMD -projectPath "$PROJECT_PATH" -executeMethod UnityEditor.EditorApplication.isPlaying &
sleep 2

echo "✅ Unity launched! Click Play in the editor to start."
