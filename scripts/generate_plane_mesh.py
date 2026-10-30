"""Generate source-space triangles split along six successive paper-plane creases.

Each polygon retains its UVs and a mask of the rigid folds that move it.
Run from the repository root after changing the crease layout.
"""
from math import sin, cos, pi
from pathlib import Path

# Vertex = source x/y, current (fully folded) x/y.
polygons = [([(-.5,-.5,-.5,-.5),(.5,-.5,.5,-.5),(.5,.5,.5,.5),(-.5,.5,-.5,.5)],0)]

def split_all(nx,ny,d,bit,reflect):
    global polygons
    result=[]
    for poly,mask in polygons:
        for side in [-1,1]:
            out=[]
            for a,b in zip(poly,poly[1:]+poly[:1]):
                da=nx*a[2]+ny*a[3]-d; db=nx*b[2]+ny*b[3]-d
                if side*da>=-1e-9: out.append(a)
                if da*db < -1e-16:
                    t=da/(da-db);out.append(tuple(x+(y-x)*t for x,y in zip(a,b)))
            clean=[]
            for v in out:
                if not clean or sum((v[k]-clean[-1][k])**2 for k in range(2))>1e-16:clean.append(v)
            if len(clean)>2 and sum((clean[0][k]-clean[-1][k])**2 for k in range(2))<1e-16:clean.pop()
            area=sum(a[0]*b[1]-a[1]*b[0] for a,b in zip(clean,clean[1:]+clean[:1]))
            if len(clean)<3 or abs(area)<1e-10:continue
            if side>0 and reflect:
                clean=[(u,v,x-2*(nx*x+ny*y-d)*nx,y-2*(nx*x+ny*y-d)*ny) for u,v,x,y in clean]
            result.append((clean,mask|(bit if side>0 else 0)))
    polygons=result

for k in range(6):
    angle=pi/(4*2**(k//2)); nx=sin(angle);ny=cos(angle)*(1 if k%2==0 else -1)
    split_all(nx,ny,.5*nx,1<<k,True)
split_all(0,1,0,64,False) # Final center ridge / wing dihedral.
triangles=[]
for poly,mask in polygons:
    for i in range(1,len(poly)-1):
        a,b,c=poly[0],poly[i],poly[i+1]
        if abs((b[0]-a[0])*(c[1]-a[1])-(b[1]-a[1])*(c[0]-a[0]))>1e-10:triangles.append((a,b,c,mask))

def uv(v):return f'vec2({v[0]+.5:.9f}, {v[1]+.5:.9f})'
lines=['// BEGIN GENERATED PLANE MESH (scripts/generate_plane_mesh.py)',f'const int PLANE_FACES = {len(triangles)};', 'void planeFace(int face, out vec2 a, out vec2 b, out vec2 c, out float folds) {']
for i,(a,b,c,mask) in enumerate(triangles):lines.append(f'  if (face == {i}) {{ a={uv(a)}; b={uv(b)}; c={uv(c)}; folds={mask}.0; return; }}')
lines+=['  a=vec2(0.0); b=a; c=a; folds=0.0;', '}', '// END GENERATED PLANE MESH']
p=Path('shaders/17-Origami.fs');s=p.read_text();start=s.index('// BEGIN GENERATED PLANE MESH');end=s.index('// END GENERATED PLANE MESH')+len('// END GENERATED PLANE MESH');p.write_text(s[:start]+'\n'.join(lines)+s[end:]);print(f'Generated {len(triangles)} triangles')
