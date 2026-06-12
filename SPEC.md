# Solar Defender - Sistema Completo

## 1. Sistema de Loja de Armas

**Armas Disponíveis:**
| Arma | Custo Base | Níveis | Dano Base | Cadência Base |
|------|------------|--------|-----------|---------------|
| Basic | Grátis (inicial) | 5 | 1-5 | 0.2s |
| Laser | 100 | 5 | 3-15 | 0.15s |
| Missile | 200 | 5 | 5-25 | 0.4s |

**Custos de Upgrade por Nível:**
- Nível 2: 50 moedas
- Nível 3: 100 moedas
- Nível 4: 200 moedas
- Nível 5: 400 moedas

## 2. Sistema de Moedas

- Inimigos SEMPRE dropam moedas ao morrer
- Quantidade base: 3-10 moedas por inimigo
- Bosses dropam 5x mais moedas
- Moedas com física magnética em direção ao jogador
- Moedas desaparecem após 10 segundos

## 3. Sistema de Combo

**Mecânica:**
- A cada kill, o combo aumenta
- Combotimeout: 3 segundos
- Multiplicador: 1 + (combo * 0.1), máximo 5x
- Score final = score base * multiplicador

**UI:**
- Painel de combo com animação
- Contador animado de kills
- Indicador de multiplicador

## 4. Sistema de Críticos

**Configurações:**
- Chance base: 10%
- Multiplicador de dano: 2x
- Duração do efeito: 0.15s

**Efeitos Visuais:**
- Texto "CRIT! X" flutuante
- Screen shake
- Hit stop (pausa do tempo)
- Flash colorido

## 5. Drone Companion

**Características:**
- Segue o jogador
- Orbita ao redor do jogador
- Auto-target e dispara em inimigos próximos
- Dano: 1 por tiro
- Cadência: 0.3s
-_RANGE: 15 unidades

**Visual:**
- Esfera verde com glow
- Partículas de thruster
- Pulsação suave

## 6. Efeitos Visuais

**Screen Effects:**
- Screen shake em explosões
- Hit stop ao acertar críticos
- Slow motion (30% velocidade)
- Vignette pulsante em dano
- Overlays de dano/crit/shield

**Hit Effects:**
- Partículas de impacto por tipo de bala
- Explosão de morte com cor do inimigo
- Trails nas balas

**UI Animations:**
- Combo popup com bounce
- Score/coins com pulse
- Level up effect
- Transições suaves

## 7. Arquivos Criados

| Arquivo | Função |
|---------|--------|
| `WeaponData.cs` | ScriptableObject de armas |
| `WeaponShopController.cs` | Controlador da loja |
| `CoinDropController.cs` | Drop de moedas |
| `WeaponShopItem.cs` | Item da loja |
| `ComboSystem.cs` | Sistema de combo |
| `CriticalHitSystem.cs` | Sistema de críticos |
| `DroneController.cs` | Drone companion |
| `GameEffectsManager.cs` | Efeitos visuais globais |
| `HitEffects.cs` | Efeitos de hit |
| `BulletEffects.cs` | Efeitos nas balas |
| `DamagePopup.cs` | Números de dano flutuantes |

## 8. Controles

- **W/A/S/D** - Movimento
- **Mouse** - Mirar
- **Click** - Atirar
- **1/2/3** - Trocar armas
- **Q** - Abrir/fechar loja
- **ESC** - Pausar
