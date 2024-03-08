import {BooleanHelpers} from "@helpers/boolean_helpers";

export const rabbitConfig = {
    HOST: process.env.RABBITMQ_HOST || 'rabbitmq',
    USER: process.env.RABBITMQ_USER || 'guest',
    PASSWORD: process.env.RABBITMQ_PASSWORD || 'guest',
    QUEUE_NAME: process.env.RABBITMQ_QUEUE_NAME || 'ingested',
    DURABLE: BooleanHelpers.parseBool(process.env.RABBITMQ_DURABLE, true),

    get RABBIT_URI(): string {
        return `amqp://${this.USER}:${this.PASSWORD}@${this.HOST}?heartbeat=30`;
    }
};
