# Solar Defender - Sistema Completo

## Sistemas Implementados

### 1. Sistema de Dodge/Roll
- **Tecla Espaço** - Roll com invencibilidade
- Distância: 5 unidades
- Duração: 0.3s
- Cooldown: 1s
- Trail visual durante dodge
- Invencibilidade durante 0.25s

### 2. Sistema de Ataque Corpo a Corpo
- **Tecla V** - Ataque melee
- Range: 2 unidades
- Dano: 50
- Trail visual da arma
- Efeito de hit

### 3. Sistema de Granadas
- **Tecla G** - Lançar granada
- Trajetória parabólica com preview
- Explosão com raio de 5 unidades
- Dano: 100 (com falloff)
- Máximo: 5 granadas

### 4. Menu Principal e Pause
- **Menu Principal**: Novo Jogo, Continuar, Carregar, Configurações, Controles, Sair
- **Menu Pause**: Continuar, Reiniciar, Configurações, Menu Principal
- **Tecla ESC** - Pause/Resume
- Transições animadas

### 5. Sistema de XP e Level
- XP por kills e missões
- Level up a cada 100 XP (multiplicador 1.5x)
- Pontos de skill a cada level
- Bônus de +10 HP a cada 5 levels

### 6. Árvore de Habilidades
- **Tecla K** - Abrir/Fechar
- Habilidades de: Dano, HP, Velocidade, Cooldown, Especial
- Pré-requisitos para habilidades avançadas
- Visualização de nodes bloqueados/desbloqueados

### 7. Post-Processing
- Bloom dinâmico
- Vignette (pior em baixa vida)
- Color grading
- Perfil de combate (bloom aumentado)
- Perfil de baixa vida

### 8. Sistema de Música Dinâmica
- Música do menu principal
- Música de exploração
- Música de combate
- Música de boss
- Música de vitória/derrota
- Crossfade suave entre músicas

### 9. Sistema de Conquistas
- Por kills, fases, bosses, moedas
- Popup de conquista desbloqueada
- Som de conquista
- Progresso salvo

### 10. Mini-Mapa
- Indicadores de planetas
- Posição do jogador (centro)
- Dots de inimigos
- Indicador de boss
- Indicador de direção do objetivo

### 11. Sistema de Fases de Boss
- Múltiplas fases por boss
- Transições com warning
- Mudança de stats por fase
- Slow motion na transição

## Controles Completos

| Tecla | Ação |
|-------|------|
| W/A/S/D | Movimento |
| Mouse | Mirar |
| Click | Atirar |
| R | Recarregar |
| Espaço | Dodge/Roll |
| V | Ataque melee |
| G | Lançar granada |
| Q | Loja de armas |
| K | Árvore de habilidades |
| Tab | Inventário |
| E | Mercador |
| ESC | Pause |

## Estrutura de Capítulos

| Capítulo | Planeta | Boss | HP | Fases |
|----------|---------|------|-----|-------|
| 1 | Mercúrio | Scout Commander | 100 | 2 |
| 2 | Vênus | Drone Lord | 150 | 2 |
| 3 | Marte | Alien Commander | 200 | 3 |
| 4 | Júpiter | Giant Commander | 300 | 3 |
| 5 | Saturno | Destroyer Prime | 400 | 4 |
| 6 | Netuno | Final Boss | 500 | 5 |

## Arquivos Criados

| Arquivo | Função |
|---------|--------|
| `DodgeRollSystem.cs` | Roll, melee, granadas |
| `MainMenuManager.cs` | Menus principal e pause |
| `PlayerProgression.cs` | XP, level, skill tree |
| `PostProcessingEffects.cs` | Post-processing, música, conquistas |
| `MiniMapSystem.cs` | Mini-mapa, fases de boss |
| `EnhancedBossMeshGenerator.cs` | Modelos de bosses |
| `GraphicsEnhancer.cs` | Sistema de gráficos |
| `WeaponMeshGenerator.cs` | Modelos de armas |
| `WeaponAmmoSystem.cs` | Sistema de munição |
| `InterplanetaryBoss.cs` | Boss interplanetário |
| `ChapterManager.cs` | Gerenciador de capítulos |
| `CutsceneManager.cs` | Sistema de cutscenes |
| `BossFactory.cs` | Factory de bosses |
| `MerchantItemsDatabase.cs` | Database do mercador |
| `HerbMixingSystem.cs` | Sistema de mistura de ervas |
| `ItemDropSystem.cs` | Sistema de drops |
| `ComboSystem.cs` | Sistema de combo |
| `CriticalHitSystem.cs` | Sistema de críticos |
| `DroneController.cs` | Drone companion |
| `GameEffectsManager.cs` | Efeitos visuais |
| `HitEffects.cs` | Efeitos de hit |
| `BulletEffects.cs` | Efeitos nas balas |
| `DamagePopup.cs` | Números de dano |
| `WeaponData.cs` | ScriptableObject de armas |
| `WeaponShopController.cs` | Controlador da loja |
| `CoinDropController.cs` | Drop de moedas |
| `WeaponShopItem.cs` | Item da loja |
| `MerchantUIController.cs` | UI do mercador |
| `ChapterDefinitions.cs` | Definições de capítulos |

## Configurações no Unity

### Para Ativar Post-Processing:
1. Instale o pacote "Post Processing" via Package Manager
2. Adicione `PostProcessingEffects` à câmera principal
3. Crie Post Process Profiles (normal, combat, lowHealth)

### Para Ativar Música:
1. Adicione `DynamicMusicSystem` a um GameObject
2. Configure AudioClips para cada estado
3. Configure AudioSource com loop enabled

### Para Ativar Skill Tree:
1. Crie painel UI com SkillNodes
2. Configure `availableSkills` list
3. Adicione `PlayerProgression` component

### Para Ativar Mini-Mapa:
1. Crie RawImage para o mapa
2. Adicione `MiniMapSystem` component
3. Configure `planetIndicators` array
