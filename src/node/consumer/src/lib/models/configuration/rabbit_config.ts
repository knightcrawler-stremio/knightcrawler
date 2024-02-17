export const rabbitConfig = {
    RABBIT_URI: process.env.RABBIT_URI || 'amqp://localhost',
    QUEUE_NAME: process.env.QUEUE_NAME || 'test-queue'
};