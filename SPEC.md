# Solar Defender - Sistema Completo

## Sistema de Carregamento de Armas

### Mecânica de Reload
- **Tecla R** - Recarregar arma
- Tempo de reload: 2 segundos
- Barra de progresso durante reload
- **Não recarrega se:**
  - Carregador cheio
  - Sem munição de reserva
  - Já está recarregando

### Sistema de Munição
| Arma | Carregador | Reserva Máxima |
|------|------------|----------------|
| Glock 17 | 17 balas | 200 |
| Doze Shotgun | 8 cartuchos | 100 |
| Minigun | 100 balas | 500 |
| Uzi | 30 balas | 200 |

## Sistema de Gráficos Aprimorados

### Bosses - Modelos Detalhados

| Boss | Estrutura | Efeitos Visuais |
|------|-----------|-----------------|
| **Scout Commander** | Corpo esférico + cabeça + 8 tentáculos segmentados + 4 olhos compostos + 6 espinhos | Anéis de corpo, orbs flutuantes, glow nos olhos |
| **Drone Lord** | Núcleo + 6 placas hexagonais + cúpula + 4 jatos hover | Glow interno ciano, linhas de energia, sensor eye |
| **Alien Commander** | Torso + ombros + cabeça + mandíbulas + braços bio-mecânicos | Crista cranial, orbs flutuantes, spines nas costas |
| **Giant Commander** | Corpo massivo + 5 olhos cluster + 3 bocas + braços/pernas colossais | Spikes por todo corpo, detritos flutuantes |
| **Destroyer Prime** | Casco + ponte + 6 pods + 4 exaustores + asas | Janelas de vidro, luzes de navegação, antenas |
| **Final Boss** | Core geométrico + 6 esferas orbitais + 8 espinhos + 8 tentáculos | Anéis de energia, aura de partículas, orbs de energia |

### Armas - Modelos Detalhados

| Arma | Partes |
|------|--------|
| **Glock 17** | Slide com serrilhas, coronha, cão, mira, carregador, cano |
| **Shotgun** | Cano com ribs, pump com grooves, receiver, stock curvo, freio de boca |
| **Uzi** | Cano, corpo, coronha dobrável, grip, carregador, mira |
| **Minigun** | 6 canos, housing do motor, estrutura, grip, pés de apoio, mira |

### Efeitos Visuais

| Efeito | Descrição |
|--------|-----------|
| **Bloom** | Glow em olhos, jatos, anéis de energia |
| **Idle Animation** | Bosses flutuam e rotacionam suavemente |
| **Pulsing Effect** | Olhos pulsam com intensidade variada |
| **Blinking Effect** | Luzes de navegação piscam |
| **Trail Renderer** | Tiros deixam rastros |
| **Muzzle Flash** | Flash ao atirar |
| **Hit Sparks** | Faíscas ao acertar inimigos |
| **Blood/Metal Splatter** | Efeitos de impacto orgânicos/metálicos |

### Animações de Boss
- **Bob** - Subir e descer suavemente
- **Rotate** - Rotação constante
- **Tilt** - Inclinação baseada no movimento
- **Floating** - Objetos flutuam ao redor

### Cores dos Bosses
| Boss | Cor |
|------|-----|
| Scout Commander | Vermelho escuro (0.6, 0.2, 0.2) |
| Drone Lord | Cinza metálico (0.4, 0.4, 0.5) |
| Alien Commander | Verde púrpura (0.3, 0.5, 0.2) |
| Giant Commander | Laranja acastanhado (0.5, 0.3, 0.2) |
| Destroyer Prime | Azul escuro (0.2, 0.3, 0.5) |
| Final Boss | Roxo (0.4, 0.1, 0.5) |

## Sistema de Capítulos e Chefes

### Estrutura de Capítulos
| Capítulo | Planeta | Boss | HP | Recompensa |
|----------|---------|------|-----|------------|
| 1 | Mercúrio | Scout Commander | 100 | R$200 |
| 2 | Vênus | Drone Lord | 150 | R$300 |
| 3 | Marte | Alien Commander | 200 | R$500 |
| 4 | Júpiter | Giant Commander | 300 | R$750 |
| 5 | Saturno | Destroyer Prime | 400 | R$1000 |
| 6 | Netuno | Final Boss | 500 | R$2000 |

## Sistema de Ervas (RE4)
| Erva | Efeito |
|------|--------|
| Verde | +10 HP |
| Vermelha | +30 HP |
| Verde+Vermelha | +50 HP |
| Verde+Amarela+Vermelha | +100 HP |

## Arquivos Criados

| Arquivo | Função |
|---------|--------|
| `EnhancedBossMeshGenerator.cs` | Modelos detalhados de bosses com efeitos |
| `GraphicsEnhancer.cs` | Configurações globais de gráficos |
| `WeaponMeshGenerator.cs` | Modelos detalhados de armas |
| `WeaponAmmoSystem.cs` | Sistema de munição e reload |
| `InterplanetaryBoss.cs` | Boss interplanetário |
| `ChapterManager.cs` | Gerenciador de capítulos |
| `CutsceneManager.cs` | Sistema de cutscenes |
| `BossFactory.cs` | Factory para criar bosses |
| `MerchantItemsDatabase.cs` | Database do mercador |
| `HerbMixingSystem.cs` | Sistema de mistura de ervas |
| `ItemDropSystem.cs` | Sistema de drops |

## Controles
- **W/A/S/D** - Movimento
- **Mouse** - Mirar/Atirar
- **R** - Recarregar arma
- **Q** - Loja
- **Tab** - Inventário
- **E** - Mercador
- **ESC** - Pausar
