﻿namespace IronJS

open System
open IronJS

module Hosting =

  let createEnvironment () =
    let x = IjsEnv()

    x.Object_methods <- {
      GetProperty = Api.ObjectModule.Property.get'
      HasProperty = Api.ObjectModule.Property.has'
      DeleteProperty = Api.ObjectModule.Property.delete'
      PutBoxProperty = Api.ObjectModule.Property.putBox'
      PutRefProperty = Api.ObjectModule.Property.putRef'
      PutValProperty = Api.ObjectModule.Property.putVal'

      GetIndex = Api.ObjectModule.Index.get'
      HasIndex = Api.ObjectModule.Index.has'
      DeleteIndex = Api.ObjectModule.Index.delete'
      PutBoxIndex = Api.ObjectModule.Index.putBox'
      PutRefIndex = Api.ObjectModule.Index.putRef'
      PutValIndex = Api.ObjectModule.Index.putVal'

      Default = Api.ObjectModule.defaultValue'
    }

    x.Base_Class <- PropertyMap(x)

    x.Prototype_Class <- Api.PropertyClass.subClass(x.Base_Class, "constructor")
    x.Function_Class <- Api.PropertyClass.subClass(x.Base_Class, ["length"; "prototype"])
    x.Array_Class <- Api.PropertyClass.subClass(x.Base_Class, "length")
    x.String_Class <- Api.PropertyClass.subClass(x.Base_Class, "length")
    x.Number_Class <- x.Base_Class
    x.Boolean_Class <- x.Base_Class

    x.Object_prototype <- Native.Object.createPrototype x
    x.Function_prototype <- Native.Function.createPrototype x
    x.Array_prototype <- Native.Array.createPrototype x
    x.String_prototype <- Native.String.createPrototype x
    x.Number_prototype <- Native.Number.createPrototype x
    x.Boolean_prototype <- Native.Boolean.createPrototype x
    
    Native.Global.setup x
    Native.Math.setup x
    Native.Object.setupPrototype x
    Native.Object.setupConstructor x
    Native.Function.setupPrototype x

    //Boxed bools
    x.Boxed_False.Bool  <- false
    x.Boxed_False.Type  <- TypeCodes.Bool
    x.Boxed_True.Bool   <- true
    x.Boxed_True.Type   <- TypeCodes.Bool

    //Boxed doubles
    x.Boxed_NegOne.Double <- -1.0
    x.Boxed_NegOne.Type   <- TypeCodes.Number
    x.Boxed_Zero.Double   <- 0.0
    x.Boxed_Zero.Type     <- TypeCodes.Number
    x.Boxed_One.Double    <- 1.0
    x.Boxed_One.Type      <- TypeCodes.Number
    x.Boxed_NaN.Double    <- System.Double.NaN
    x.Boxed_NaN.Type      <- TypeCodes.Number

    //Boxed null
    x.Boxed_Null.Clr  <- null
    x.Boxed_Null.Type <- TypeCodes.Clr

    //Boxed empty string
    x.Boxed_EmptyString.Clr   <- ""
    x.Boxed_EmptyString.Type  <- TypeCodes.String

    //Boxed undefined
    x.Boxed_Undefined.Clr   <- Undefined.Instance
    x.Boxed_Undefined.Type  <- TypeCodes.Undefined

    //Temp boxes
    x.Temp_Bool.Type      <- TypeCodes.Bool
    x.Temp_Number.Type    <- TypeCodes.Number
    x.Temp_Clr.Type       <- TypeCodes.Clr
    x.Temp_String.Type    <- TypeCodes.String
    x.Temp_Function.Type  <- TypeCodes.Function
    x.Temp_Object.Type    <- TypeCodes.Object

    x

  type Context(env:IjsEnv) =
    
    let globalFunc = new IronJS.Function(env)

    member x.Environment = env
    member x.GlobalFunc = globalFunc

    member x.CompileFile fileName =
      let tree = Ast.Parsers.Ecma3.parseGlobalFile fileName
      let analyzed = Ast.applyAnalyzers tree None
      Compiler.Core.compileAsGlobal env analyzed

    member x.CompileSource source =
      let tree = Ast.Parsers.Ecma3.parseGlobalSource source
      let analyzed = Ast.applyAnalyzers tree None
      Compiler.Core.compileAsGlobal env analyzed

    member x.InvokeCompiled (compiled:Delegate) =
      let result = compiled.DynamicInvoke(globalFunc, env.Globals)
      Utils.unboxObj result

    member x.ExecuteFile fileName = x.InvokeCompiled (x.CompileFile fileName)
    member x.ExecuteFileT<'a> fileName = x.ExecuteFile fileName :?> 'a
    member x.Execute source = x.InvokeCompiled (x.CompileSource source)
    member x.ExecuteT<'a> source = x.Execute source :?> 'a

    member x.PutGlobal (name, value:obj) =
      env.Globals.Methods.PutBoxProperty.Invoke(env.Globals, name, Utils.box value)

    member x.GetGlobal name =
      env.Globals.Methods.GetProperty.Invoke(env.Globals, name)

    member x.CreateDelegateFunction delegate' =
      Api.DelegateFunction.create env delegate'

    static member Create () =
      new Context(createEnvironment())