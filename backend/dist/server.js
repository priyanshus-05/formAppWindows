"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const express_1 = __importDefault(require("express"));
const body_parser_1 = __importDefault(require("body-parser"));
const fs_1 = __importDefault(require("fs"));
const path_1 = __importDefault(require("path"));
const app = (0, express_1.default)();
const port = 3000;
const dbPath = path_1.default.resolve(__dirname, 'db.json');
app.use(body_parser_1.default.json());
// Endpoint to check server status
app.get('/ping', (req, res) => {
    res.send(true);
});
// Endpoint to save a submission
app.post('/submit', (req, res) => {
    const { name, email, phone, github_link, stopwatch_time } = req.body;
    const newSubmission = { name, email, phone, github_link, stopwatch_time };
    fs_1.default.readFile(dbPath, 'utf8', (err, data) => {
        if (err) {
            return res.status(500).send('Error reading database file');
        }
        const db = JSON.parse(data);
        db.submissions.push(newSubmission);
        fs_1.default.writeFile(dbPath, JSON.stringify(db, null, 2), (err) => {
            if (err) {
                return res.status(500).send('Error saving submission');
            }
            res.status(200).send('Submission saved successfully');
        });
    });
});
// Endpoint to retrieve a submission
app.get('/read', (req, res) => {
    const index = parseInt(req.query.index, 10);
    fs_1.default.readFile(dbPath, 'utf8', (err, data) => {
        if (err) {
            return res.status(500).send('Error reading database file');
        }
        const db = JSON.parse(data);
        if (index >= 0 && index < db.submissions.length) {
            res.status(200).json(db.submissions[index]);
        }
        else {
            res.status(404).send('Submission not found');
        }
    });
});
// Initialize the database file if it doesn't exist
if (!fs_1.default.existsSync(dbPath)) {
    fs_1.default.writeFileSync(dbPath, JSON.stringify({ submissions: [] }, null, 2));
}
app.listen(port, () => {
    console.log(`Server is running on http://localhost:${port}`);
});
