import express, { Request, Response } from 'express';
import bodyParser from 'body-parser';
import fs from 'fs';
import path from 'path';

const app = express();
const port = 3000;
const dbPath = path.resolve(__dirname, 'db.json');

app.use(bodyParser.json());

// Endpoint to check server status
app.get('/ping', (req: Request, res: Response) => {
  res.send(true);
});

// Endpoint to save a submission
app.post('/submit', (req: Request, res: Response) => {
  const { name, email, phone, github_link, stopwatch_time } = req.body; // Ensure these names match the Submission class

  // Log the received data for debugging
  console.log('Received data:', { name, email, phone, github_link, stopwatch_time });

  const newSubmission = { name, email, phone, github_link, stopwatch_time };

  fs.readFile(dbPath, 'utf8', (err, data) => {
    if (err) {
      return res.status(500).send('Error reading database file');
    }
    const db = JSON.parse(data);
    db.submissions.push(newSubmission);
    fs.writeFile(dbPath, JSON.stringify(db, null, 2), (err) => {
      if (err) {
        return res.status(500).send('Error saving submission');
      }
      res.status(200).send('Submission saved successfully');
    });
  });
});

// Endpoint to retrieve a submission
app.get('/read', (req: Request, res: Response) => {
  const index = parseInt(req.query.index as string, 10);
  console.log(`Received request to read submission at index: ${index}`);

  fs.readFile(dbPath, 'utf8', (err, data) => {
    if (err) {
      return res.status(500).send('Error reading database file');
    }
    const db = JSON.parse(data);
    console.log(`Database contents: ${JSON.stringify(db)}`);

    if (index >= 0 && index < db.submissions.length) {
      console.log(`Returning submission: ${JSON.stringify(db.submissions[index])}`);
      res.status(200).json(db.submissions[index]);
    } else {
      console.log(`Submission not found at index: ${index}`);
      res.status(404).send('Submission not found');
    }
  });
});

// Endpoint to retrieve all submissions
app.get('/submissions', (req: Request, res: Response) => {
  fs.readFile(dbPath, 'utf8', (err, data) => {
    if (err) {
      return res.status(500).send('Error reading database file');
    }
    const db = JSON.parse(data);
    res.status(200).json(db.submissions);
  });
});

// Endpoint to delete a submission
app.delete('/delete', (req: Request, res: Response) => {
  const index = parseInt(req.query.index as string, 10);
  console.log(`Received request to delete submission at index: ${index}`);

  fs.readFile(dbPath, 'utf8', (err, data) => {
    if (err) {
      return res.status(500).send('Error reading database file');
    }
    const db = JSON.parse(data);

    if (index >= 0 && index < db.submissions.length) {
      db.submissions.splice(index, 1);
      fs.writeFile(dbPath, JSON.stringify(db, null, 2), (err) => {
        if (err) {
          return res.status(500).send('Error deleting submission');
        }
        res.status(200).send('Submission deleted successfully');
      });
    } else {
      res.status(404).send('Submission not found');
    }
  });
});

// Endpoint to update a submission
app.put('/update', (req: Request, res: Response) => {
  const index = parseInt(req.query.index as string, 10);
  const { name, email, phone, github_link, stopwatch_time } = req.body;

  console.log(`Received request to update submission at index: ${index}`);

  fs.readFile(dbPath, 'utf8', (err, data) => {
    if (err) {
      return res.status(500).send('Error reading database file');
    }
    const db = JSON.parse(data);

    if (index >= 0 && index < db.submissions.length) {
      db.submissions[index] = { name, email, phone, github_link, stopwatch_time };
      fs.writeFile(dbPath, JSON.stringify(db, null, 2), (err) => {
        if (err) {
          return res.status(500).send('Error updating submission');
        }
        res.status(200).send('Submission updated successfully');
      });
    } else {
      res.status(404).send('Submission not found');
    }
  });
});

// Initialize the database file if it doesn't exist
if (!fs.existsSync(dbPath)) {
  fs.writeFileSync(dbPath, JSON.stringify({ submissions: [] }, null, 2));
}

// Endpoint to search for a submission by email
app.get('/search', (req: Request, res: Response) => {
  const email = req.query.email as string;

  fs.readFile(dbPath, 'utf8', (err, data) => {
    if (err) {
      return res.status(500).send('Error reading database file');
    }
    const db = JSON.parse(data);
    const submission = db.submissions.find((submission: any) => submission.email === email);
    if (submission) {
      res.status(200).json(submission);
    } else {
      res.status(404).send('Submission not found');
    }
  });
});

app.listen(port, () => {
  console.log(`Server is running on http://localhost:${port}`);
});
