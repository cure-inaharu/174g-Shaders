/*{
  "ISFVSN":"2",
  "CATEGORIES":["Geometry Adjustment","Stylize"],
  "DESCRIPTION":"Fold into an origami crane or paper plane, depart upper-right, return from lower-left and unfold. Transparent outside the paper.",
  "INPUTS":[
    {"NAME":"inputImage","TYPE":"image"},
    {"NAME":"mode","LABEL":"Mode","TYPE":"long","VALUES":[0,1],"LABELS":["Origami Crane","Paper Plane"],"DEFAULT":0},
    {"NAME":"cycleSeconds","LABEL":"Cycle (seconds)","TYPE":"float","DEFAULT":9.0,"MIN":2.0,"MAX":30.0},
    {"NAME":"craneSize","LABEL":"Paper Size","TYPE":"float","DEFAULT":0.58,"MIN":0.25,"MAX":1.0},
    {"NAME":"planeWidth","LABEL":"Plane Width","TYPE":"float","DEFAULT":1.8,"MIN":1.0,"MAX":3.0},
    {"NAME":"wingMotion","LABEL":"Motion (flap / glide)","TYPE":"float","DEFAULT":0.65,"MIN":0.0,"MAX":1.0},
    {"NAME":"foldShade","LABEL":"Fold Shading","TYPE":"float","DEFAULT":0.65,"MIN":0.0,"MAX":1.0},
    {"NAME":"autoFly","LABEL":"Auto Fly","TYPE":"bool","DEFAULT":true},
    {"NAME":"progress","LABEL":"Manual Progress","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0}
  ]
}*/

float ease(float x) { x=clamp(x,0.0,1.0); return x*x*(3.0-2.0*x); }
float cross2(vec2 a,vec2 b) { return a.x*b.y-a.y*b.x; }

// Clockwise perimeter of the unfolded sheet; four corners are included exactly.
vec2 sourceRing(int i) {
  float t=float(i)/3.0;
  if(i<3) return vec2(0.0,t);
  if(i<6) return vec2(t-1.0,1.0);
  if(i<9) return vec2(1.0,3.0-t);
  return vec2(4.0-t,0.0);
}

// Tail, far wing, long raised neck, bent head, beak and near wing.
// These are 3D paper facets, projected and depth-tested in the fragment shader.
vec3 craneRing(int i,float flap) {
  if(i==0) return vec3(-0.72,0.14,0.02);
  if(i==1) return vec3(-0.20,0.10,0.04);
  if(i==2) return vec3(-0.34,0.55+0.12*flap,-0.24+0.30*flap);
  if(i==3) return vec3(0.13,0.13,-0.04);
  if(i==4) return vec3(0.24,0.11,0.02);
  if(i==5) return vec3(0.40,0.53,0.04);
  if(i==6) return vec3(0.53,0.52,0.06);
  if(i==7) return vec3(0.65,0.34,0.06);
  if(i==8) return vec3(0.44,0.42,0.08);
  if(i==9) return vec3(0.22,-0.08,0.08);
  if(i==10) return vec3(-0.09,-0.52+0.13*flap,0.30+0.26*flap);
  return vec3(-0.29,-0.12,0.06);
}

vec3 craneCenter(int face) {
  if(face>=5 && face<=7) return vec3(0.43,0.44,0.07);
  if(face>=4 && face<=8) return vec3(0.24,0.03,0.07);
  return vec3(-0.02,0.00,0.16);
}

// BEGIN GENERATED PLANE MESH (scripts/generate_plane_mesh.py)
const int PLANE_FACES = 18;
void planeFace(int face, out vec2 a, out vec2 b, out vec2 c, out float folds) {
  if (face == 0) { a=vec2(1.000000000, 0.500000000); b=vec2(0.000000000, 0.500000000); c=vec2(0.000000000, 0.301087633); folds=0.0; return; }
  if (face == 1) { a=vec2(1.000000000, 0.500000000); b=vec2(0.000000000, 0.698912367); c=vec2(0.000000000, 0.500000000); folds=64.0; return; }
  if (face == 2) { a=vec2(1.000000000, 0.500000000); b=vec2(0.000000000, 0.301087633); c=vec2(0.000000000, 0.085786438); folds=32.0; return; }
  if (face == 3) { a=vec2(1.000000000, 0.500000000); b=vec2(0.000000000, 0.914213562); c=vec2(0.000000000, 0.698912367); folds=80.0; return; }
  if (face == 4) { a=vec2(0.251697119, 0.000000000); b=vec2(0.500000000, 0.000000000); c=vec2(1.000000000, 0.500000000); folds=8.0; return; }
  if (face == 5) { a=vec2(0.000000000, 0.000000000); b=vec2(0.251697119, 0.000000000); c=vec2(1.000000000, 0.500000000); folds=40.0; return; }
  if (face == 6) { a=vec2(0.000000000, 0.000000000); b=vec2(1.000000000, 0.500000000); c=vec2(0.000000000, 0.085786438); folds=40.0; return; }
  if (face == 7) { a=vec2(1.000000000, 0.500000000); b=vec2(0.500000000, 1.000000000); c=vec2(0.251697119, 1.000000000); folds=68.0; return; }
  if (face == 8) { a=vec2(1.000000000, 0.500000000); b=vec2(0.251697119, 1.000000000); c=vec2(0.000000000, 1.000000000); folds=84.0; return; }
  if (face == 9) { a=vec2(1.000000000, 0.500000000); b=vec2(0.000000000, 1.000000000); c=vec2(0.000000000, 0.914213562); folds=84.0; return; }
  if (face == 10) { a=vec2(0.900543816, 0.000000000); b=vec2(1.000000000, 0.000000000); c=vec2(1.000000000, 0.500000000); folds=2.0; return; }
  if (face == 11) { a=vec2(0.792893219, 0.000000000); b=vec2(0.900543816, 0.000000000); c=vec2(1.000000000, 0.500000000); folds=34.0; return; }
  if (face == 12) { a=vec2(0.500000000, 0.000000000); b=vec2(0.665910681, 0.000000000); c=vec2(1.000000000, 0.500000000); folds=10.0; return; }
  if (face == 13) { a=vec2(0.665910681, 0.000000000); b=vec2(0.792893219, 0.000000000); c=vec2(1.000000000, 0.500000000); folds=42.0; return; }
  if (face == 14) { a=vec2(1.000000000, 0.500000000); b=vec2(1.000000000, 1.000000000); c=vec2(0.900543816, 1.000000000); folds=65.0; return; }
  if (face == 15) { a=vec2(1.000000000, 0.500000000); b=vec2(0.900543816, 1.000000000); c=vec2(0.792893219, 1.000000000); folds=81.0; return; }
  if (face == 16) { a=vec2(1.000000000, 0.500000000); b=vec2(0.665910681, 1.000000000); c=vec2(0.500000000, 1.000000000); folds=69.0; return; }
  if (face == 17) { a=vec2(1.000000000, 0.500000000); b=vec2(0.792893219, 1.000000000); c=vec2(0.665910681, 1.000000000); folds=85.0; return; }
  a=vec2(0.0); b=a; c=a; folds=0.0;
}
// END GENERATED PLANE MESH

bool foldBit(float mask,float bit) { return mod(floor(mask/bit),2.0)>0.5; }

vec3 hinge(vec3 p,vec3 origin,vec3 axis,float angle) {
  vec3 v=p-origin;
  return origin+v*cos(angle)+cross(axis,v)*sin(angle)+axis*dot(axis,v)*(1.0-cos(angle));
}

vec3 planePoint(vec2 uv,float mask,float fold,float aspect,float size) {
  vec3 p=vec3(uv-0.5,0.0);
  // Start directly with the corners, then fold alternating long edges.
  for(int k=0;k<6;++k) {
    float angle=0.785398163/pow(2.0,floor(float(k)/2.0));
    float side=mod(float(k),2.0)<0.5 ? 1.0 : -1.0;
    vec3 axis=vec3(-cos(angle)*side,sin(angle),0.0);
    float amount=ease((fold-float(k)*0.12)/0.12);
    if(foldBit(mask,pow(2.0,float(k)))) {
      p=hinge(p,vec3(0.5,0.0,0.0),axis,-3.14159265*amount);
      p.z+=0.0003*amount;
    }
  }
  // Close the center fold, then open the two rigid wings into a shallow V.
  float close=ease((fold-0.73)/0.12);
  float openWings=ease((fold-0.85)/0.15);
  float tilt=foldBit(mask,64.0) ? mix(2.7,0.35,openWings)*close : -0.35*openWings;
  p=hinge(p,vec3(0.0),vec3(1.0,0.0,0.0),tilt);
  float settle=ease(fold);
  // Shorter nose and broader wings give the plane a stockier silhouette.
  // Apply the proportions progressively so the open image retains its full frame.
  float fit=1.0/sqrt(max(planeWidth,1.0));
  p.x*=mix(aspect,size*1.65*0.82*fit,settle);
  p.y*=mix(1.0,size*1.65*planeWidth*fit,settle);
  p.z*=mix(1.0,size*1.65*fit,settle);
  return p;
}

vec3 paperPoint(vec2 uv,vec3 target,float fold,float aspect,float size) {
  vec3 unfolded=vec3((uv-0.5)*vec2(aspect,1.0),0.0);
  // Lift the folds out of the sheet before settling into the folded shape.
  vec3 p=mix(unfolded,target*size,fold);
  p.z+=sin(fold*3.14159265)*0.22*abs(uv.x-0.5);
  return p;
}

vec3 pose(vec3 p,float bank,float scale) {
  float c=cos(bank),s=sin(bank);
  p.xy=mat2(c,s,-s,c)*p.xy;
  return p*scale;
}

void main() {
  float phase=autoFly ? fract(TIME/max(cycleSeconds,0.01)) : clamp(progress,0.0,1.0);
  float fold=ease((phase-0.08)/0.22)*(1.0-ease((phase-0.85)/0.15));
  if(fold<0.00001) { gl_FragColor=IMG_NORM_PIXEL(inputImage,isf_FragNormCoord); return; }
  float aspect=RENDERSIZE.x/max(RENDERSIZE.y,1.0);
  float size=craneSize*min(aspect,1.25);
  float flightClock=phase*cycleSeconds;
  float flap=sin(flightClock*7.0)*wingMotion*ease((phase-0.23)/0.10)*(1.0-ease((phase-0.82)/0.06));
  // A plane glides with a slow bank; its wings stay rigid.
  float motion=mode==1 ? sin(flightClock*2.1)*wingMotion*0.55 : flap;
  float outTravel=pow(clamp((phase-0.43)/0.23,0.0,1.0),2.0);
  float inTravel=pow(1.0-clamp((phase-0.69)/0.16,0.0,1.0),2.0);
  // Both endpoints are entirely beyond the canvas, so the wrap is invisible.
  vec2 extent=vec2(aspect*0.5+size+0.25,0.5+size+0.25);
  vec2 offset=phase<0.68 ? extent*outTravel : -extent*inTravel;
  float travel=phase<0.68 ? outTravel : inTravel;
  float flightScale=mix(1.0,0.74,travel);
  float bank=fold*((mode==1 ? 0.43 : 0.10)+0.055*motion+0.12*travel);
  offset.y+=0.012*motion*fold*(1.0-travel);
  vec2 pixel=(isf_FragNormCoord-0.5)*vec2(aspect,1.0)-offset;
  vec4 result=vec4(0.0);
  float nearest=-100.0;
  for(int panel=0;panel<24;++panel) {
    if(mode==1 && panel>=PLANE_FACES) break;
    int i=panel<12 ? panel : panel-12;
    int j=i==11 ? 0 : i+1;
    vec2 ua=vec2(0.5),ub=sourceRing(i),uc=sourceRing(j);
    vec3 ta=craneCenter(i),tb=craneRing(i,flap),tc=craneRing(j,flap);
    // Crease gussets keep the neck and body joined as their fold centers separate.
    if(panel>=12) {
      int previous=i==0 ? 11 : i-1;
      ub=vec2(0.5); uc=sourceRing(i);
      tb=craneCenter(previous); tc=craneRing(i,flap);
    }
    vec3 a=pose(paperPoint(ua,ta,fold,aspect,size),bank,flightScale);
    vec3 b=pose(paperPoint(ub,tb,fold,aspect,size),bank,flightScale);
    vec3 c=pose(paperPoint(uc,tc,fold,aspect,size),bank,flightScale);
    if(mode==1) {
      float folds;
      planeFace(panel,ua,ub,uc,folds);
      a=pose(planePoint(ua,folds,fold,aspect,size),bank,flightScale);
      b=pose(planePoint(ub,folds,fold,aspect,size),bank,flightScale);
      c=pose(planePoint(uc,folds,fold,aspect,size),bank,flightScale);
    }
    vec3 q=1.0/(vec3(1.0)-vec3(a.z,b.z,c.z)*0.45);
    vec2 pa=a.xy*q.x,pb=b.xy*q.y,pc=c.xy*q.z;
    float area=cross2(pb-pa,pc-pa);
    if(abs(area)<0.0000001) continue;
    vec3 w;
    w.y=cross2(pixel-pa,pc-pa)/area;
    w.z=cross2(pb-pa,pixel-pa)/area;
    w.x=1.0-w.y-w.z;
    if(min(w.x,min(w.y,w.z)) < 0.0) continue;
    w*=q; w/=w.x+w.y+w.z;
    float depth=dot(w,vec3(a.z,b.z,c.z));
    // Folded paper layers can coincide; stable ties prevent surface flicker.
    if(depth<nearest+(mode==1 ? 0.00001 : 0.0)) continue;
    nearest=depth;
    vec2 uv=ua*w.x+ub*w.y+uc*w.z;
    vec4 paper=IMG_NORM_PIXEL(inputImage,clamp(uv,0.0,1.0));
    vec3 normal=normalize(cross(b-a,c-a));
    bool backFacing=normal.z<0.0;
    if(normal.z<0.0) normal=-normal;
    float light=0.48+0.52*max(dot(normal,normalize(vec3(-0.4,0.5,1.0))),0.0);
    // Separate pale reverse-side facets, rather than a gradient over the image.
    bool reverseFace=(panel>=12 || i==0 || (i>=4 && i<=8) || i==10);
    if(mode==1) reverseFace=backFacing;
    // Print shows through the plane's reverse side, keeping the input recognizable.
    if(reverseFace) paper.rgb=mix(paper.rgb,vec3(0.89,0.90,0.92)*paper.a,mode==1 ? 0.28 : ease((fold-0.25)/0.65));
    paper.rgb*=mix(1.0,light,foldShade*fold);
    result=paper;
  }
  gl_FragColor=result;
}
