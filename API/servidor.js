// servidor.js
const express = require("express");
const cors = require("cors");
const { Sequelize, DataTypes } = require("sequelize");
const app = express();
app.use(cors());

// Conexión con MySQL
const sequelize = new Sequelize("lugares_db", "root", "BetoRoot.26", {
  host: "localhost",
  dialect: "mysql",
  port: 3306,
  logging: false, // Opcional: para no mostrar las consultas SQL en consola
});

// Definición del modelo
const Lugar = sequelize.define("Lugar", {
  id: {
    type: DataTypes.INTEGER,
    autoIncrement: true,
    primaryKey: true,
  },
  nombre: {
    type: DataTypes.STRING,
    allowNull: false,
  },
  descripcion: {
    type: DataTypes.STRING,
    allowNull: false,
  },
  fecha_visita: {
    type: DataTypes.DATE,
    allowNull: false,
  },
  URL_lugar: {
    type: DataTypes.STRING,
    allowNull: false,
  },
});

// Probar conexión y sincronizar la BD
async function inicializarDB() {
  try {
    await sequelize.authenticate();
    console.log("Conexión a MySQL establecida correctamente ✅");
    
    await sequelize.sync();
    console.log("Base de datos y tabla de lugares listas 🚀");
  } catch (error) {
    console.error("Error al conectar con la base de datos:", error);
  }
}

inicializarDB();

// Rutas GET ----------------------------
// Obtener todos los lugares
app.get("/lugares", async (req, res) => {
  try {
    const lugares = await Lugar.findAll();
    res.json(lugares);
  } catch (error) {
    res.status(500).json({ error: "Error al obtener lugares", detalle: error.message });
  }
});

// Agregar lugar
app.get("/agregar", async (req, res) => {
  const { nombre, descripcion, fecha_visita, URL_lugar } = req.query;
  if (!nombre || !descripcion || !fecha_visita || !URL_lugar) {
    return res.status(400).json({ error: "Faltan parámetros" });
  }
  try {
    const nuevo = await Lugar.create({ nombre, descripcion, fecha_visita, URL_lugar });
    res.json({ mensaje: "Lugar agregado", lugar_visitado: nuevo });
  } catch (error) {
    res.status(500).json({ error: "Error al agregar el lugar", detalle: error.message });
  }
});

// Editar lugar
app.get("/editar", async (req, res) => {
  const { id, nombre, descripcion, fecha_visita, URL_lugar } = req.query;
  if (!id) return res.status(400).json({ error: "Falta id" });
  try {
    const lugar = await Lugar.findByPk(id);
    if (!lugar) return res.status(404).json({ error: "Lugar no encontrado" });

    lugar.nombre = nombre || lugar.nombre;
    lugar.descripcion = descripcion || lugar.descripcion;
    lugar.fecha_visita = fecha_visita || lugar.fecha_visita;
    lugar.URL_lugar = URL_lugar || lugar.URL_lugar;
    await lugar.save();
    res.json({ mensaje: "Lugar actualizado", lugar_visitado: lugar });
  } catch (error) {
    res.status(500).json({ error: "Error al editar el lugar", detalle: error.message });
  }
});

// Eliminar lugar
app.get("/eliminar", async (req, res) => {
  const { id } = req.query;
  if (!id) return res.status(400).json({ error: "Falta id" });
  try {
    const lugar = await Lugar.findByPk(id);
    if (!lugar) return res.status(404).json({ error: "Lugar no encontrado" });
    await lugar.destroy();
    res.json({ mensaje: "Lugar eliminado" });
  } catch (error) {
    res.status(500).json({ error: "Error al eliminar el lugar", detalle: error.message });
  }
});

// Servidor
const PORT = 3000;
app.listen(PORT, () => {
  console.log(`Servidor corriendo en http://localhost:${PORT}`);
});