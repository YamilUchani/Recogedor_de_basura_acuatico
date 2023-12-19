# Cross-View Gait Recognition Based on U-Net

<div align="center" style="background-color: #6495ED; padding: 10px;">
  <a href="#description" style="color: #FF00FF;"><b>Description</b></a> |
  <a href="#our-approach" style="color: #FF00FF;"><b>Our Approach</b></a> |
  <a href="#results" style="color: #FF00FF;"><b>Results</b></a> |
  <a href="#getting-started" style="color: #FF00FF;"><b>Getting Started</b></a> |
  <a href="#citing" style="color: #FF00FF;"><b>Citing</b></a>
</div>



Last Updated: April 27th, 2021

---

## Description

Inspired by the successes of GANs in image translation tasks, this project proposes a gait recognition technique using a conditional generative model to generate view-invariant features. The method is evaluated on the CASIA gait database B, showcasing outstanding performance, especially in carrying-bag and wearing-coat sequences.

## Our Approach

### Framework

![Framework](.readme/Framework.svg)

### Conditional GAN (CGAN)

#### Generator
![Generator](.readme/U-Gait2.svg)

#### Discriminator
![Discriminator](.readme/Discriminator2.svg)

## Results

Qualitative results include original and generated Gait Energy Image (GEI) representations, along with a comparison with other approaches based on the correct classification rate (CCR).

- Original GEI representations for subject 120:
  ![Original GEI](.readme/Subject120OriginalGEI.png)

- Generated GEI representations for subject 120:
  ![Generated GEI](.readme/Subject120GeneratedGEI.png)

- Generated GEI representations from multiple subjects:
  ![Generated GEI](.readme/Gen_mul.gif)

- Comparison with other approaches based on CCR:
  ![CCR Comparison](.readme/PlotCCR.svg)

## Getting Started

You can find the notebook [here](notebooks/Gait_U_Net2.ipynb) or open it on [Google Colab](https://colab.research.google.com/drive/1GXSScKJ5uOJLZ-9aseO3vXLYen_DLJ9p#forceEdit=true&sandboxMode=true).

### Prerequisites

- Tensorflow 2.x
- Keras
- OpenCV
- Numpy
- Matplotlib

## Built With

- [CASIA](http://www.cbsr.ia.ac.cn/english/Gait%20Databases.asp) - The dataset used
- [Pix2Pix](https://www.tensorflow.org/tutorials/generative/pix2pix) - Based on
- GoogleColab - The virtual machine used in the experiments

## Citing

Please, cite this work as follows:

