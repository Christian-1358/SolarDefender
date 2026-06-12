# Solar Defender - Sistema Completo de Mercador

## 1. Sistema de Armas

### Armas Comerciáveis
| Arma | Preço | Dano | Cadência | Carregador |
|------|-------|------|----------|------------|
| Glock 17 | R$200 | 15 | 0.15s | 17 |
| Glock 17 Fire | R$350 | 20 | 0.15s | 17 |
| Doze Shotgun | R$500 | 80 | 0.8s | 8 |
| Minigun | R$1500 | 8 | 0.05s | 100 |
| Uzi | R$600 | 10 | 0.08s | 30 |

### Munições
| Munição | Preço |
|---------|-------|
| Glock (17) | R$50 |
| Shotgun (8) | R$100 |
| Minigun (100) | R$300 |
| Uzi (30) | R$120 |

## 2. Sistema de Cura

### Injeções
| Item | Preço | Efeito |
|------|-------|--------|
| Injeção de Cura | R$150 | +50 HP |
| Injeção Max | R$500 | +100 HP (cura completa) |
| Injeção de Escudo | R$200 | +25 Escudo |

## 3. Sistema de Ervas (Estilo Resident Evil 4)

### Ervas Individuais
| Erva | Preço | Efeito |
|------|-------|--------|
| Erva Verde | R$30 | +10 HP |
| Erva Vermelha | R$50 | +30 HP |
| Erva Amarela | R$40 | +5 HP (potencializa) |
| Erva Azul | R$60 | Efeito especial |

### Receitas de Mistura
| Combinação | Resultado | Efeito |
|------------|-----------|--------|
| Verde + Amarela | Erva Verde+Amarela | +25 HP |
| Verde + Vermelha | Erva Verde+Vermelha | +50 HP |
| Verde + Azul | Erva Verde+Azul | +20 HP + 15 Escudo |
| Verde+Amarela + Vermelha | Erva Verde+Amarela+Vermelha | +100 HP (cura máxima) |

## 4. Sistema de Drops

### Drops de Inimigos
- **Moedas**: 80% chance, 5-20 unidades
- **Munição**: 40% chance, 1-3 unidades
- **Ervas**: 25% chance, 1-2 unidades

### Loot Tables Personalizadas
Inimigos podem ter loot tables específicas com:
- Chances de drop customizadas
- Quantidades mínimas/máximas
- Tipos específicos de munição/ervas

## 5. Inventário

### Mochila (Backpack)
- Slots iniciais: 12
- Máximo: 48
- Custos de upgrade: 100, 250, 500, 1000

### Categorias de Itens
- **Armas**: Armas compradas
- **Munição**: Balas para armas
- **Recuperação**: Injeções e itens de cura
- **Ervas**: Ervas para crafting

## 6. Arquivos Criados

| Arquivo | Função |
|---------|--------|
| `MerchantItemsDatabase.cs` | Database de todos os itens do mercador |
| `MerchantUIController.cs` | UI completa do mercador com abas |
| `HerbMixingSystem.cs` | Sistema de mistura de ervas estilo RE4 |
| `ItemDropSystem.cs` | Sistema de drops de munição, ervas e moedas |

## 7. Controles

- **Tab/I** - Abrir inventário
- **E** - Interagir com mercador
- **Q** - Abrir loja de armas (sistema anterior)

## 8. UI do Mercador

### Abas
1. **Armas** - Lista de armas disponíveis
2. **Munição** - Balas para cada arma
3. **Recuperação** - Injeções e curas
4. **Ervas** - Ervas para mistura
5. **Misturar** - Abre painel de crafting

### Painel de Info
- Nome do item
- Descrição
- Preço
- Estatísticas
- Botão Comprar
