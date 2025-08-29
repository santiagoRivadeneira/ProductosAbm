<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Productos.aspx.cs" Inherits="ProductosAbm.Pages.Productos" %>
<%@ Register Src="~/Pages/ProductosTable.ascx" TagPrefix="uc" TagName="ProductosTable" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Formulario Maestro de Productos</title>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.6.0/jquery.min.js"></script>
    <link href="~/Content/Productos.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/productos.js"></script>
</head>

<body>
    <div id="panelContainer">
        <h2>Formulario Maestro de Productos</h2>

  
        <div id="formAdd">
            <input type="text" id="codigo" placeholder="Código del producto"/>
            <input type="text" id="nombre" placeholder="Nombre del producto"/>
            <input type="text" id="unidad" placeholder="Unidad de medida"/>
            <input type="number" id="precio" step="0.01" placeholder="Precio"/>
            <select id="estado">
                <option value="true">Activo</option>
                <option value="false">Inactivo</option>
            </select>
            <button id="btnAdd" type="button">Agregar Producto</button>
        </div>

        <div id="snackbar"></div>

   
        <uc:ProductosTable ID="ProductosTable1" runat="server" />

        <div style="margin-top: 20px; text-align: center;">
            <div id="pagination"></div>
        </div>
   </div>
    

</body>
</html>
