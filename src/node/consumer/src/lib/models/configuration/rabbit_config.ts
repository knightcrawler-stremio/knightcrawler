export class RabbitConfig {
    public RABBIT_URI: string = process.env.RABBIT_URI || 'amqp://localhost';
    public QUEUE_NAME: string = process.env.QUEUE_NAME || 'test-queue';
}