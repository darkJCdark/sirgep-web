# SIRGEP - Sistema Integral de Reservas y Gestión de Espacios Públicos

SIRGEP es una plataforma web que permite a ciudadanos reservar espacios municipales y comprar entradas a eventos públicos. El sistema está diseñado con una arquitectura modular y segura, desplegado en entornos Ubuntu (backend) y Windows (frontend).

---

## 🖥️ Backend

- **Tecnologías**: Java 21, Apache NetBeans IDE 21, Maven, SOAP Web Services.
- **Estructura modular** (`SirgepSolution/`):
  - `SirgepDomain`: entidades del sistema.
  - `SirgepDBManager`: conexión y configuración de base de datos.
  - `SirgepDA`: capa de acceso a datos (DAO).
  - `SirgepBusiness`: lógica de negocio.
  - `SirgepWS`: servicios web expuestos vía SOAP.
- **Funciones**:
  - Gestión de usuarios: clientes registrados, invitados y administradores.
  - Creación y administración de eventos y espacios.
  - Envío de correos automáticos desde `sirgep.oficial@gmail.com`.
  - Dashboard de ventas y calendario de reservas.
  - Descarga de comprobantes en PDF.
  - Registro de reservas mediante archivos Excel.
- **Seguridad**:
  - Credenciales cifradas con **AES**.
  - Llaves protegidas con **MD5**, desencriptadas con **ChaCha20**.
  - Archivo `token-properties` para manejo seguro de claves.

---

## 🎨 Frontend

- **Tecnologías**: Visual Studio Community 2022, ASP.NET Web Forms (ASPx), C#, Bootstrap.
- **Estructura funcional**:
  - `Presentacion/`: interfaz principal.
  - `Infraestructura/`: módulos de `Espacio`, `Evento`, `Ubicacion`.
  - `Usuarios/`: `Administrador`, `Comprador`.
  - `Ventas/`: `Entrada`, `Reserva`.
  - `Reportes/`: generación de informes.
- **Funciones**:
  - Inicio de sesión y registro.
  - Acceso como invitado.
  - Búsqueda por departamento, provincia y distrito.
  - Compra de entradas y reserva de espacios.
  - Métodos de pago: tarjeta, Yape, Plin.
  - Descarga de comprobantes PDF con logo oficial.
  - Visualización de imágenes y calendario de reservas.

---

## 🗄️ Base de Datos

- **Motor**: MySQL Workbench 8.0 CE.
- **Infraestructura**: instancia en **AWS Academy**.
- **Modelo**: basado en diagrama de clases (Draw.io).
- **Seguridad**: cifrado de credenciales y llaves con AES, MD5 y ChaCha20.

---

## 🚀 Despliegue

- **Backend**: Ubuntu.
- **Frontend**: Windows.
- **Documentación**: almacenada en Google Drive (Docs y Presentación en PPT).

---

## 📊 Funcionalidades clave

- Reservas y compras en línea.
- Notificaciones automáticas por correo.
- Métodos de pago múltiples.
- Dashboard de ventas y calendario de reservas.
- Descarga de comprobantes PDF.
- Exportación de reservas y entradas en Excel.

---

## 📧 Contacto

Correo oficial de notificaciones: **sirgep.oficial@gmail.com**
