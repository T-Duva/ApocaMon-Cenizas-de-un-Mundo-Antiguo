# NavMesh 2D (plano XY) – Setup con NavMeshPlus

El NavMesh nativo de Unity hornea en el plano **XZ**. Para un Tower Defense 2D (vista top-down en **XY**) se usa el paquete **NavMeshPlus**, que permite bake en XY y que los enemigos caminen sobre el suelo y respeten obstáculos.

## 1. Paquete instalado

En `Packages/manifest.json` está añadido:

- `com.h8man.2d.navmeshplus` (GitHub: h8man/NavMeshPlus)

Tras abrir el proyecto, Unity descargará el paquete. Si no aparece, en **Window > Package Manager** usa **+ > Add package from git URL** e introduce:

`https://github.com/h8man/NavMeshPlus.git`

## 2. Objeto NavMesh en la escena (ej. 03_Batalla)

1. Crear un **GameObject vacío** en la raíz (ej. nombre: `NavMesh`).
2. Añadir componentes:
   - **Navigation Surface** (NavMeshSurface del paquete).
   - **Navigation Collect Sources 2d** (NavMeshPlus).
3. En **Navigation Collect Sources 2d** pulsar **"Rotate Surface to XY"** (orienta la superficie al plano 2D).
4. Dejar ese objeto seleccionado para el paso del Bake.

## 3. Suelo (walkable)

- El objeto **Suelo** (Sprite en XY) debe tener:
  - **SpriteRenderer** (ya lo tienes).
  - **Navigation Modifier** (Add Component > Navigation Modifier).
  - En Navigation Modifier: **no** marcar "Override Area" (o área por defecto Walkable).
- Opción con colliders: si usas **Use Geometry > Physics Colliders** en la Navigation Surface, el Suelo debe tener un **Collider2D** (BoxCollider2D, etc.). Con **Render Meshes** se usa la malla del sprite (recomendado para un único sprite de suelo).

## 4. Paredes / obstáculos (no walkable)

- En cada objeto que sea pared u obstáculo:
  - Añadir **Navigation Modifier**.
  - Activar **Override Area** y en **Area Type** elegir **Not Walkable**.

Defensas (ApocaMonDefensa con NavMesh Obstacle + Carve) siguen funcionando como obstáculos dinámicos.

## 5. Bake

- Seleccionar el GameObject que tiene **Navigation Surface** + **Navigation Collect Sources 2d**.
- En el Inspector, en **Navigation Surface**, pulsar **Bake**.

Deberías ver el mesh de navegación en el plano XY (suelo verde, huecos donde hay obstáculos).

## 6. Enemigos y Meta

- **GeneradorEnemigos** ya asigna a cada enemigo:
  - `agent.updateRotation = false`
  - `agent.updateUpAxis = false`
  - `agent.SetDestination(meta.position)`
- Así los agentes se mantienen en el plano 2D y van hacia la **Meta** que tengas asignada en el Inspector.

## Si el Bake sale vacío

- Comprobar que en **Navigation Collect Sources 2d** hayas pulsado **Rotate Surface to XY** antes del Bake.
- Que el **Suelo** tenga **Navigation Modifier** (y, si usas Physics Colliders, un **Collider2D** en el Suelo).
- Que la **Navigation Surface** tenga definido un **Agent Type** (por defecto suele valer).

## Referencia

- [NavMeshPlus – HOW TO](https://github.com/h8man/NavMeshPlus/wiki/HOW-TO)
- Tu suelo (Sprite en XY) + NavMeshPlus sustituyen el uso del NavMesh nativo en XZ; los enemigos caminan sobre el suelo actual y esquivan obstáculos y defensas.
