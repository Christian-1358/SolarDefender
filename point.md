# Solar Defender - Resumo do Projeto

## O que é
**Solar Defender** é um jogo de defesa espacial 3D desenvolvido em Unity, onde o jogador controla a última frota de defesa da Terra contra invasores alienígenas.

## Conceito
- **Gênero**: Space shooter / Tower defense
- **Objetivo**: Defender cada planeta do sistema solar até o confronto final em Netuno
- **Progressão**: 6 fases (planetas) + 3 chefes épicos

## Estrutura do Projeto

### Sistemas Principais
| Sistema | Descrição |
|---------|-----------|
| GameManager | Singleton que gerencia estado global do jogo |
| EnemySpawner | Sistema de spawn com 6 tipos de inimigos + 3 boss |
| AdvancedAI | IA avançada com patrol, chase, flanking e retreat |
| DatabaseManager | Persistência JSON para saves e configurações |
| SkillTree | Árvore de habilidades desbloqueáveis |
| CraftingSystem | Sistema de crafts |
| FirstPersonMode | Modo primeira pessoa com armas |
| AnimationSystem | 5 subsistemas de animação (Ship, Boss, UI, Effects, etc.) |

### Conteúdo do Jogo
- **6 Planetas**: Mercúrio → Vênus → Marte → Júpiter → Saturno → Netuno
- **6 Tipos de Inimigos**: Scout, Fighter, Tank, Mother, Destroyer, Commander
- **3 Bosses**: AlienCommander, GiantCommander, FinalBoss
- **5 Armas**: Glock, Shotgun, Rifle, Sniper, RocketLauncher
- **5-6 Habilidades**: Escudo, Turbo, Nuke, etc.
- **20+ Achievements**

### Tecnologias
- **Engine**: Unity 2021.3+
- **Linguagem**: C# (98 scripts, ~23.500 linhas)
- **Persistência**: JSON (Mono.Data.Sqlite mencionado)
- **UI**: TextMeshPro

## Como Executar
1. Abrir projeto no Unity Hub
2. Importar TextMeshPro (se necessário)
3. Abrir cena `Assets/Scenes/MainScene.unity`
4. Pressionar Play (Ctrl+P)

## Controles
| Tecla | Ação |
|-------|------|
| W/A/S/D | Mover nave |
| Mouse | Mirar |
| Click | Atirar |
| 1/2/3 | Trocar arma |
| Q | Abrir loja |
| F | Toggle First Person |
| Tab | Inventário |
| ESC | Pausar |

---
*Projeto criado em 2026*
