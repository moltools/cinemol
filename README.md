[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](./LICENSE)
[![Maintainer](https://img.shields.io/badge/Maintainer-davidmeijer-blue)](https://github.com/davidmeijer)
[![Generic badge](https://img.shields.io/badge/Version-alpha-green.svg)](https://shields.io/)

# CineMol

<img src="./logo.png" alt="logo" width="100">

CineMol is a direct-to-SVG small molecule drawer. 

You can install CineMol with pip from the source of this repository:

```bash
pip install .
```

## Usage Python

```python
from cinemol.parsers import parse_sdf 
from cinemol.drawer import draw_molecule

atoms, bonds = parse_sdf('path/to/your/molecule.sdf')
svg_str = draw_molecule(atoms, bonds)
```

## Usage CLI

```bash
cinemol -i path/to/your/molecule.sdf -o path/to/your/molecule.svg
```

Run `cinemol -h` for more options.
