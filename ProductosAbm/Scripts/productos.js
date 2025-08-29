// ----------------------------
// Variables globales
// ----------------------------
let currentPage = 1;
const pageSize = 5;

// ----------------------------
// Mostrar snackbar
// ----------------------------
function showSnackbar(message, isError = false) {
    const snackbar = $('#snackbar');
    snackbar.text(message)
        .css('background-color', isError ? '#dc3545' : '#28a745')
        .addClass('show');
    setTimeout(() => snackbar.removeClass('show'), 3000);
}


// ----------------------------
// Cargar productos con paginación
// ----------------------------
function loadProducts(page = 1) {
    currentPage = page;
    $.ajax({
        type: "POST",
        url: "Productos.aspx/GetProductos",
        data: JSON.stringify({ pageNumber: page, pageSize: pageSize }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            const result = response.d;
            const products = result.products;
            const total = result.total;

            const tableBody = $("#productosTable tbody");
            tableBody.empty();

            if (products.length === 0) {
                tableBody.append('<tr><td colspan="6">No hay productos</td></tr>');
            } else {
                products.forEach(p => {
                    tableBody.append(`
                        <tr data-id="${p.Id}">
                            <td class="codigo">${p.Codigo}</td>
                            <td class="nombre">${p.Nombre}</td>
                            <td class="unidad">${p.Unidad}</td>
                            <td class="precio">${p.Precio.toFixed(2)}</td>
                            <td class="estado">${p.Estado ? 'Activo' : 'Inactivo'}</td>
                            <td>
                                <button class="btnEdit">Editar</button>
                                <button class="btnDelete">Eliminar</button>
                            </td>
                        </tr>
                    `);
                });
            }

            // Paginación
            const totalPages = Math.ceil(total / pageSize);
            const paginationDiv = $("#pagination");
            paginationDiv.empty();
            for (let i = 1; i <= totalPages; i++) {
                const btn = $(`<button>${i}</button>`);
                if (i === currentPage) btn.attr("disabled", true);
                btn.click(() => loadProducts(i));
                paginationDiv.append(btn);
            }
        },
        error: function () {
            showSnackbar("Error al cargar productos", true);
        }
    });
}

// ----------------------------
// Delegación de eventos para editar en línea
// ----------------------------
$(document).on('click', '.btnEdit', function () {
    const row = $(this).closest('tr');
    startEdit(row);
});

// ----------------------------
// Función para editar en línea
// ----------------------------
function startEdit(row) {
    // Cancelar edición en otras filas
    $('#productosTable tbody tr').each(function () {
        if ($(this).find('.btnCancel').length) cancelEdit($(this));
    });

    // Guardar valores originales
    const original = {
        codigo: row.find('.codigo').text(),
        nombre: row.find('.nombre').text(),
        unidad: row.find('.unidad').text(),
        precio: row.find('.precio').text(),
        estado: row.find('.estado').text() === 'Activo'
    };
    row.data('original', original);

    // Inputs con estilo
    row.find('.codigo').html('<input class="edit-input" type="text" value="' + original.codigo + '"/>');
    row.find('.nombre').html('<input class="edit-input" type="text" value="' + original.nombre + '"/>');
    row.find('.unidad').html('<input class="edit-input" type="text" value="' + original.unidad + '"/>');
    row.find('.precio').html('<input class="edit-input" type="number" step="0.01" value="' + original.precio + '"/>');
    row.find('.estado').html(
        '<select class="edit-input"><option value="true"' + (original.estado ? ' selected' : '') + '>Activo</option>' +
        '<option value="false"' + (!original.estado ? ' selected' : '') + '>Inactivo</option></select>'
    );

    // Cambiar botón Editar a Guardar
    const editBtn = row.find('.btnEdit');
    editBtn.text('Guardar').off('click').on('click', function () {
        saveEdit(row);
    });

    // Ocultar botón Eliminar mientras se edita
    row.find('.btnDelete').hide();

    // Crear o mostrar botón Cancelar
    let cancelBtn = row.find('.btnCancel');
    if (cancelBtn.length === 0) {
        cancelBtn = $('<button class="btn btn-sm btn-danger btnCancel">Cancelar</button>');
        editBtn.after(cancelBtn);
    }
    cancelBtn.show().off('click').on('click', function () {
        cancelEdit(row);
    });
}

// ----------------------------
// Cancelar edición
// ----------------------------
function cancelEdit(row) {
    const original = row.data('original');
    if (!original) return;

    // Restaurar valores originales
    row.find('.codigo').text(original.codigo);
    row.find('.nombre').text(original.nombre);
    row.find('.unidad').text(original.unidad);
    row.find('.precio').text(parseFloat(original.precio).toFixed(2));
    row.find('.estado').text(original.estado ? 'Activo' : 'Inactivo');

    // Restaurar botón Editar
    row.find('.btnEdit').text('Editar').off('click').on('click', function () {
        startEdit(row);
    });

    // Ocultar botón Cancelar
    row.find('.btnCancel').hide();

    // Mostrar botón Eliminar
    row.find('.btnDelete').show();

    // Limpiar datos originales
    row.removeData('original');
}

// ----------------------------
// Guardar edición
// ----------------------------
function saveEdit(row) {
    const id = row.data('id');
    const updatedProduct = {
        Id: id,
        Codigo: row.find('.codigo input').val(),
        Nombre: row.find('.nombre input').val(),
        Unidad: row.find('.unidad input').val(),
        Precio: parseFloat(row.find('.precio input').val()),
        Estado: row.find('.estado select').val() === 'true'
    };

    $.ajax({
        type: "POST",
        url: "Productos.aspx/UpdateProducto",
        data: JSON.stringify({ product: updatedProduct }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            showSnackbar(response.d);
            loadProducts(currentPage);
        },
        error: function () {
            showSnackbar("Error al actualizar producto", true);
        }
    });
}

// ----------------------------
// Agregar producto
// ----------------------------
$(document).on("click", "#btnAdd", function (e) {
    e.preventDefault();
    const codigo = $('#codigo').val().trim();
    const nombre = $('#nombre').val().trim();
    const unidad = $('#unidad').val().trim();
    const precio = parseFloat($('#precio').val());
    const estado = $('#estado').val() === 'true';

    if (!codigo) { showSnackbar("El código es obligatorio", true); return; }
    if (!nombre) { showSnackbar("El nombre es obligatorio", true); return; }
    if (!unidad) { showSnackbar("La unidad es obligatoria", true); return; }
    if (isNaN(precio) || precio <= 0) { showSnackbar("El precio debe ser mayor a 0", true); return; }

    $.ajax({
        type: "POST",
        url: "Productos.aspx/AddProducto",
        data: JSON.stringify({ product: { Codigo: codigo, Nombre: nombre, Unidad: unidad, Precio: precio, Estado: estado } }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            showSnackbar(response.d);
            loadProducts(currentPage);
            $('#codigo,#nombre,#unidad,#precio').val('');
            $('#estado').val('true');
        },
        error: function (xhr) {
            const message = xhr.responseJSON?.Message || "Ocurrió un error";
            showSnackbar(message, true);
        }
    });
});

// ----------------------------
// Eliminar producto
// ----------------------------
$(document).on('click', '.btnDelete', function () {
    const row = $(this).closest('tr');
    const id = row.data('id');
    if (!confirm("¿Desea eliminar este producto?")) return;

    $.ajax({
        type: "POST",
        url: "Productos.aspx/DeleteProducto",
        data: JSON.stringify({ id: id }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            showSnackbar(response.d);
            loadProducts(currentPage);
        },
        error: function () {
            showSnackbar("Error al eliminar producto", true);
        }
    });
});

// ----------------------------
// Inicializar tabla al cargar página
// ----------------------------
$(document).ready(function () {
    loadProducts(currentPage);
});
