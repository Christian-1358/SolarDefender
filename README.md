# Solar Defender - Unity Project

## 📖 Descrição

Jogo de defesa espacial 3D onde você é o comandante da última frota de defesa da Terra. Invasores alienígenas estão conquistando planeta por planeta. Defenda cada planeta do sistema solar até o confronto final em Netuno!

## 🎮 Gameplay

- **6 Fases** (planetas): Mercúrio → Vênus → Marte → Júpiter → Saturno → Netuno
- **3 Chefes**: AlienCommander, GiantCommander, FinalBoss
- **Sistema de Armas**: Tiro básico, Laser, Míssil teleguiado
- **Sistema de Upgrades**: Motor, Escudo, HP Max, Armas especiais
- **Power-ups**: Vida, Escudo, Moedas, Armas, Nuclear

## 🛠️ Como Configurar no Unity

### Requisitos
- Unity 2021.3 ou superior
- TextMeshPro (Package Manager)

### Instalação

1. **Clone ou copie este projeto** para uma pasta

2. **Abra o Unity Hub** → Clique em **"Open"** → Selecione a pasta `SolarDefender`

3. **Importar TextMeshPro** (se não estiver):
   - Window → Package Manager
   - Search "TextMeshPro"
   - Install

4. **Configurar Cena Principal**:
   - Abra a cena `Assets/Scenes/MainScene.unity`
   - Ou crie uma nova cena vazia

5. **Criar Pasta "Prefabs"** se não existir:
   - Botão direito → Create → Folder → nomeie como "Prefabs"

### 🎯 Configuração dos Objetos

#### 1. Sol
- Create → 3D Object → Sphere (nome: "Sun")
- Scale: (5, 5, 5)
- Adicione script `SunController`
- Adicione Light (Point Light) como child

#### 2. Planetas
Para cada planeta (Mercúrio, Vênus, Marte, Júpiter, Saturno, Netuno):
- Create → 3D Object → Sphere
- Scale baseado no tamanho do planeta
- Adicione script `PlanetController`
- Configure PlanetData (Create → SolarDefender → PlanetData)
- Adicione ao array `planetPositions` no EnemySpawner

#### 3. Nave do Jogador
- Crie um prefab com:
  - Body: Cone ou modelo 3D
  - Wings: Box ou modelo
  - Engine: Sphere com emissive material
- Adicione tag "Player"
- Adicione script `PlayerController`
- Arraste para cena

#### 4. Inimigos
Crie prefabs para cada tipo:
- Scout (pequeno, vermelho)
- Fighter (médio, laranja)
- Tank (grande, roxo)
- Mother (muito grande, magenta)
- Destroyer (gigante, vermelho escuro)
- Commander (chefão)

Para cada prefab:
- Adicione tag "Enemy"
- Adicione script `EnemyController`
- Adicione MeshCollider

#### 5. Balas
- Bullet (esfera ciano)
- LaserBullet (esfera vermelha)
- Missile (esfera laranja, opcional Trail Renderer)
- Adicione script `BulletController`
- Adicione tag "Bullet"

#### 6. Asteroides
- Dodecahedron ou Icosahedron
- Adicione script `AsteroidController`
- Tag: "Asteroid"

#### 7. Power-ups
- Octahedron
- Adicione script `PowerupController`
- Cores diferentes para cada tipo

#### 8. Sistema Solar (Pai)
- Empty GameObject chamado "SolarSystem"
- Adicione scripts:
  - `SunController`
  - `StarfieldController`
- Adicione as planetas como children

#### 9. Spawners
- Empty GameObject "EnemySpawner"
  - Adicione script `EnemySpawner`
  - Arraste os prefabs de inimigos
  - Arraste as posições dos planetas

- Empty GameObject "AsteroidSpawner"
  - Adicione script `AsteroidSpawner`
  - Arraste prefab de asteroides

#### 10. GameManager
- Empty GameObject "GameManager"
  - Adicione script `GameManager`
  - Arraste o array de PlanetData

#### 11. UIManager
- Canvas com UI:
  - TextMeshProUGUI para textos
  - Images para barras
  - Buttons para menus
- Adicione script `UIManager`
- Conecte todas as referências

### 📁 Estrutura de Pastas Sugerida

```
Assets/
├── Scripts/
│   ├── Managers/
│   │   └── GameManager.cs
│   ├── Player/
│   │   └── PlayerController.cs
│   ├── Enemies/
│   │   └── EnemyController.cs
│   ├── UI/
│   │   └── UIManager.cs
│   └── Game/
│       ├── BulletController.cs
│       ├── EnemySpawner.cs
│       ├── EnemyData.cs
│       ├── PlanetData.cs
│       ├── PowerupController.cs
│       ├── AsteroidController.cs
│       ├── AsteroidSpawner.cs
│       ├── SunController.cs
│       ├── PlanetController.cs
│       ├── StarfieldController.cs
│       ├── CameraController.cs
│       └── InputManager.cs
├── Prefabs/
│   ├── Player/
│   ├── Enemies/
│   ├── Bullets/
│   ├── Asteroids/
│   └── Powerups/
├── Scenes/
│   └── MainScene.unity
├── ScriptableObjects/
│   ├── PlanetData/
│   └── EnemyData/
├── Materials/
└── Textures/
```

### ▶️ Executando o Jogo

1. Configure todos os prefabs e referências
2. Pressione Play (Ctrl+P)
3. Clique "Iniciar Missão"

## 🎮 Controles

| Tecla | Ação |
|-------|------|
| W/A/S/D | Mover nave |
| Mouse | Mirar |
| Click | Atirar |
| 1 | Arma básica |
| 2 | Laser (se desbloqueado) |
| 3 | Míssil (se desbloqueado) |
| Q | Abrir/fechar loja |
| ESC | Pausar |
| Right Mouse | Orbitar câmera |

## 📊 Dados dos Planetas

| Planeta | Dificuldade | Inimigos | Chefão |
|---------|-------------|----------|--------|
| Mercúrio | 1 | 8 scouts | - |
| Vênus | 2 | 12 scouts+fighter | - |
| Marte | 3 | 15 mixed | AlienCommander |
| Júpiter | 5 | 20 fighter+tank+mother | - |
| Saturno | 6 | 25 tank+mother+destroyer | GiantCommander |
| Netuno | 8 | 30 mother+destroyer | FinalBoss |

## 💰 Sistema de Moedas

- Derrote inimigos: 5-50 moedas
- Complete fase: 50 + (fase × 50) moedas
- Use na loja de upgrades

## 🔧 Troubleshooting

**Erro: MissingReferenceException**
- Verifique se todos os prefabs estão arrastados nos campos corretos

**Inimigos não spawnam**
- Verifique se EnemySpawner tem referência ao PlanetPositions array

**UI não atualiza**
- Verifique se UIManager tem todas as referências configuradas

**Câmera não segue**
- Configure o target no CameraController ou deixe encontrar automaticamente

## 📊 Sistema de Banco de Dados

O jogo utiliza **SQLite** para persistência de dados local.

### Estrutura do Banco

| Tabela | Descrição |
|--------|-----------|
| Player | Dados do jogador (nome, score, kills, deaths, tempo) |
| LevelProgress | Progresso por nível (tempo, score, chefes derrotados) |
| GameSettings | Configurações de áudio, gráficos e controles |
| Leaderboard | Rankings globais |
| PlayerUpgrades | Upgrades comprados pelo jogador |
| EnemyStats | Estatísticas por tipo de inimigo |

### Scripts do Banco de Dados

```
Assets/Scripts/Database/
├── DatabaseConfig.cs       # Configuração de caminho
├── DatabaseManager.cs     # Conexão SQLite (singleton)
├── DatabaseAccess.cs      # Facade para todos os repositórios
├── Models.cs              # Modelos de dados
├── PlayerRepository.cs    # Repositório de jogador
├── LevelProgressRepository.cs
├── LeaderboardRepository.cs
├── GameSettingsRepository.cs
├── UpgradeRepository.cs
├── EnemyStatsRepository.cs
└── DatabaseBootstrapper.cs
```

### Uso

```csharp
// Acessar dados
var player = DatabaseAccess.Instance.GetOrCreatePlayer("Commander");
DatabaseAccess.Instance.Player.AddScore(player.Id, 1000);
DatabaseAccess.Instance.CompleteLevel(player.Id, 1, "Mercúrio", 120f, 5000, 8, false);

// Leaderboard
var topScores = DatabaseAccess.Instance.Leaderboard.GetTopScores(10);
int rank = DatabaseAccess.Instance.Leaderboard.GetPlayerRank(5000);

// Progresso
var progress = DatabaseAccess.Instance.LevelProgress.GetProgressByPlayerId(player.Id);
```

### Configuração no Unity

1. **Primeira cena** adicione o `DatabaseBootstrapper` a um GameObject vazio
2. O sistema inicializa automaticamente o banco na pasta `Assets/Database/`
3. Para builds standalone, o banco é criado em `Application.persistentDataPath`

### Requisito: SQLite

O projeto usa **Mono.Data.Sqlite** (incluso no Unity). Para garantir:

1. Verifique que o Assembly `Mono.Data.Sqlite` está referenciado
2. Para builds iOS, pode ser necessário modificar o `Api Compatibility Level`

## 🎯 Sistemas Implementados

### Sistema de Animações
```
Assets/Scripts/Animation/
├── AnimationManager.cs    # Tweens: move, scale, rotate, fade, shake
├── ShipAnimator.cs        # Nave: idle, thrust, damage, boost, death
├── EnemyAnimator.cs      # Inimigos: idle, move, attack, damage, death
├── BossAnimator.cs       # Chefes: entry épico, fases, enrage, morte dramática
├── UIAnimator.cs        # UI: hover, click, panels, progress bars, typewriter
└── EffectsAnimator.cs    # Efeitos: explosões, impactos, power-ups, damage numbers
```

### Sistema de Áudio
```
Assets/Scripts/Audio/
└── AudioManager.cs    # Música, SFX, voz, volume separado
```

### Sistema de Achievements
```
Assets/Scripts/Achievements/
├── AchievementData.cs    # 20+ conquistas
└── AchievementManager.cs # Tracking, recompensas
```

### Sistema de Habilidades
```
Assets/Scripts/Abilities/
├── AbilityData.cs    # 6 habilidades (Escudo, Turbo, Nuke, etc)
└── AbilityManager.cs # Cooldown, energia, efeitos
```

### Sistema de Unlockables
```
Assets/Scripts/Unlockables/
├── UnlockableData.cs    # Skins, trails, efeitos
└── UnlockableManager.cs # Compra, equip, save
```

### Sistema de Desafios
```
Assets/Scripts/Challenges/
├── ChallengeData.cs    # Diários e semanais
└── ChallengeManager.cs # Tracking, recompensas
```

### Modos de Jogo
```
Assets/Scripts/GameModes/
└── GameModeManager.cs  # Story, Arcade, Survival, Speedrun, BossRush
```

### Otimização
```
Assets/Scripts/Optimization/
├── ObjectPool.cs      # Pooling de objetos
└── MobileControls.cs  # Controles touch
```

### UI/Efeitos
```
Assets/Scripts/UI/
├── Menus/
│   ├── MainMenu.cs       # Menu principal
│   ├── PauseMenu.cs      # Menu de pausa
│   ├── SettingsMenu.cs   # Configurações completas
│   └── AchievementsUI.cs
├── Effects/
│   └── PostProcessingController.cs  # Bloom, vignette, chromatic
└── LeaderboardUI.cs
```

## 🎮 Controles Completos

| Tecla | Ação |
|-------|------|
| W/A/S/D | Mover nave |
| Mouse | Mirar |
| Click | Atirar |
| 1/2/3 | Trocar arma |
| Q/E/R/T/Y/U | Habilidades |
| M | Abrir Mercador |
| ESC | Pausar |
| Right Mouse | Orbitar câmera |

## 📊 Estrutura Completa de Scripts

```
Assets/Scripts/
├── Abilities/
│   ├── AbilityData.cs
│   └── AbilityManager.cs
├── Achievements/
│   ├── AchievementData.cs
│   └── AchievementManager.cs
├── Animation/
│   ├── AnimationManager.cs    # Tweens globais
│   ├── ShipAnimator.cs       # Nave do jogador
│   ├── EnemyAnimator.cs      # Inimigos
│   ├── BossAnimator.cs       # Chefes
│   ├── UIAnimator.cs         # UI elements
│   └── EffectsAnimator.cs    # Efeitos visuais
├── Audio/
│   └── AudioManager.cs
├── Challenges/
│   ├── ChallengeData.cs
│   └── ChallengeManager.cs
├── Database/
│   ├── DatabaseAccess.cs
│   ├── DatabaseBootstrapper.cs
│   ├── DatabaseConfig.cs
│   ├── DatabaseManager.cs
│   ├── EnemyStatsRepository.cs
│   ├── GameSettingsRepository.cs
│   ├── LeaderboardRepository.cs
│   ├── LevelProgressRepository.cs
│   ├── Models.cs
│   ├── PlayerRepository.cs
│   └── UpgradeRepository.cs
├── Enemies/
│   └── EnemyController.cs
├── Game/
│   ├── AsteroidController.cs
│   ├── AsteroidSpawner.cs
│   ├── BulletController.cs
│   ├── CameraController.cs
│   ├── EnemyData.cs
│   ├── EnemySpawner.cs
│   ├── InputManager.cs
│   ├── PlanetController.cs
│   ├── PlanetData.cs
│   ├── PowerupController.cs
│   ├── StarfieldController.cs
│   └── SunController.cs
├── GameModes/
│   └── GameModeManager.cs
├── Managers/
│   └── GameManager.cs
├── Optimization/
│   ├── MobileControls.cs
│   └── ObjectPool.cs
├── Player/
│   └── PlayerController.cs
├── UI/
│   ├── Effects/
│   │   └── PostProcessingController.cs
│   ├── LeaderboardUI.cs
│   ├── Menus/
│   │   ├── AchievementsUI.cs
│   │   ├── MainMenu.cs
│   │   ├── PauseMenu.cs
│   │   └── SettingsMenu.cs
│   └── UIManager.cs
└── Unlockables/
    ├── UnlockableData.cs
    └── UnlockableManager.cs
```

## 📝 Licença

Este projeto foi criado como referência educacional.

---

*Criado por Jarvis AI - 2026*
