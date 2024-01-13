namespace Engine
open Types
    type IIntegrator =
        abstract member Integrate: string * double * double * double -> Result<double * Vertices, string>
